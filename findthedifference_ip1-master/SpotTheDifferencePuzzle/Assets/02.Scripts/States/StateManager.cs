using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum StateManagerStatus
{
    NONE, INITIALIZE, READY,  OPEN_STATE, CLOSE_STATE
}

public class StateManager : MonoBehaviour
{
    #region SINGLETON
    static StateManager _instance;
    public static StateManager Instance { get { return _instance; } }

    #endregion SINGLETON

    private StateManagerStatus _stateManagerStatus = StateManagerStatus.NONE;

    [SerializeField]
    private StateData _stateData;
    
    [SerializeField]
    private List<State> _states;

    private List<State> _unloadedStateList;

    [Header("Loading")]
    [SerializeField] SimpleSpinner _loadingSpinner;

    [Header("Top")]
    [SerializeField] UserCurrencyBar _userCurrencyBar;

    [Header("Loading Screen")]
    [SerializeField] LoadingScreen _loadingScreen;
    [SerializeField] float _loadingScreenAppearTime = 1.0f;
    [SerializeField] float _loadingScreenMinMaintainTime = 0.5f;

    [Header("Block Screen")]
    [SerializeField] RectTransform _blockScreen;

    [Header("Scene FadeInOut")]
    [SerializeField] CanvasGroup _fadeInOutCanvas;
    [SerializeField] float _fadeInOutTime;

    Stack<State> _stackedStates = new Stack<State>();

    Coroutine _coroutineStateTransition;

    int _blockScreenActiveCount = 0;

    StringBuilder _stringBuilder = new StringBuilder();

    #region Properties
    public string[] stateNames
    {
        get
        {
            List<string> names = new List<string>();
            if (_states != null)
            {
                for (int i = 0, max = _states.Count; i < max; ++i)
                {
                    if (_states[i] != null)
                    {
                        names.Add(_states[i].stateName);
                    }
                }
            }

            return names.ToArray();
        }
    }

    public bool nowTransitioning { get { return _stateManagerStatus != StateManagerStatus.READY; } }
    public bool IsBlockScreen { get { return _blockScreenActiveCount > 0; } }

    public State topState { get { return (_stackedStates.Count > 0) ? _stackedStates.Peek() : null; } }

    public StateManagerStatus StateManagerStatus { get { return _stateManagerStatus; } }

    public List<State> StatePoolList
    {
        get
        {
            return _stateData.StateList;
        }
    }

    public void StopStateTransition()
    {
        if (nowTransitioning)
        {
            StopCoroutine(_coroutineStateTransition);
            _coroutineStateTransition = null;
            _stateManagerStatus = StateManagerStatus.READY;
            ActiveBlockScreen(false);
        }
    } 
    #endregion

    #region UNITY EVENTS
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _loadingScreen.OnInitialize();
        }
        else if (_instance != this)
        {
            Destroy(this);
        }
    }

    IEnumerator CreateState(Type type)
    {
        State state = null;
        for (int i = 0; i < StatePoolList.Count; i++)
        {
            if(StatePoolList[i].GetType() == type && type != typeof(IntroState))
            {
                state =  Instantiate<State>(StatePoolList[i], Vector3.up*10000f, Quaternion.identity, transform);
                state.gameObject.SetActive(false);
                yield return state.OnInitialize();
                state.transform.localPosition = Vector3.zero;
                _states.Add(state);                
            }
        }
    }

    private IEnumerator Start()
    {
        _fadeInOutCanvas.alpha = 0f;
        ShowUserCurrencyBar(false);
        _unloadedStateList = new List<State>();

        for (int i = 0; i < _stateData.PreloadStateTypes.Count; i++)
        {
            yield return CreateState(_stateData.PreloadStateTypes[i]);
        }

        if (_states.Count <= 0)
        {
            LogStateStatus("ERROR", "Empty states");

            yield break;
        }

        yield return ProcessInitializeStates();

    }

    private void Update()
    {
        if (nowTransitioning || topState == null)
        {
            return;
        }

        topState.OnUpdate();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (nowTransitioning ||  topState == null)
        {
            return;
        }

        if (hasFocus)
        {
            topState.OnAppResume();
        }
        else
        {
            topState.OnAppPause();
        }
    }
    #endregion UNITY EVENTS

    #region Others Methods
    public void ShowUserCurrencyBar(bool isShow)
    {
        _userCurrencyBar.gameObject.SetActive(isShow);
    }

    public void ActiveBlockScreen(bool active)
    {
        LogStateStatus("ActiveBlockScreen", ">" + active + " , blockScreenActiveCount : " + _blockScreenActiveCount);
        if (active)
        {
            ++_blockScreenActiveCount;

            _blockScreen.gameObject.SetActive(true);
        }
        else
        {
            _blockScreenActiveCount = Mathf.Max(0, _blockScreenActiveCount - 1);
            if (_blockScreenActiveCount <= 0)
            {
                _blockScreen.gameObject.SetActive(false);
            }
        }
    }

    public State GetRegisteredState(Type type)
    {
        Debug.Log("GetRegisteredState type : " + type);
        string stateName = type.ToString();
        if (string.IsNullOrEmpty(stateName) || _states.Count <= 0)
        {
            return null;
        }

        for (int i = 0, max = _states.Count; i < max; ++i)
        {
            if (string.Compare(_states[i].stateName, stateName, true) == 0)
            {
                return _states[i];
            }
        }

        return null;

    }

    public bool IsStacked(Type type)
    {
        return IsStacked(type.Name);
    }

    public bool IsStacked(string stateName)
    {
        if (string.IsNullOrEmpty(stateName))
        {
            return false;
        }

        int index = _stackedStates.Count;
        foreach (State stackedState in _stackedStates)
        {
            --index;

            if (string.Compare(stackedState.stateName, stateName, true) == 0)
            {
                return (index > 0);
            }
        }

        return false;
    }

    public bool IsStacked(State state)
    {
        if (state == null)
        {
            return false;
        }

        return IsStacked(state.stateName);
    }

    #endregion

    #region Public Methods
    public void OpenPopupState<T>(Type type, T args = null) where T : BaseStateData
    {
        OpenState(type, false, args);
    }

    public void OpenState<T>(Type type, bool isShowLoadingScreen, T args = null) where T : BaseStateData
    {
        State state = GetEnableOpenState(type);

        if (state != null)
        {
            _stateManagerStatus = StateManagerStatus.OPEN_STATE;
            if (state.StateType == StateType.SCENE)
            {
                _coroutineStateTransition = StartCoroutine(ProcessOpenState(state, isShowLoadingScreen, args));
            }
            else
            {
                _coroutineStateTransition = StartCoroutine(ProcessOpenPushState(state, isShowLoadingScreen, args));
            }
        }
        else
        {
            if (StatePoolList.Find(s => s.GetType() == type))
            {
                _coroutineStateTransition = StartCoroutine(ProcessCreateAndOpenState(type, isShowLoadingScreen, args));
            }
        }
    }

    IEnumerator ProcessCreateAndOpenState<T>(Type type, bool isShowLoadingScreen, T args = null) where T : BaseStateData
    {
        yield return CreateState(type);
        State state = GetEnableOpenState(type);
        if (state.StateType == StateType.SCENE)
        {
            StartCoroutine(ProcessOpenState(state, isShowLoadingScreen, args));
        }
        else
        {
            StartCoroutine(ProcessOpenPushState(state, isShowLoadingScreen, args));
        }        
    }

    public void CloseState(Type type, bool showLoadingScreen = false)
    {
        State state = GetEnableOpenState(type);
        if (state != null)
        {
            _stateManagerStatus = StateManagerStatus.CLOSE_STATE;
            _coroutineStateTransition = StartCoroutine(ProcessCloseState(state, showLoadingScreen));
        }
    }
    private State GetEnableOpenState(Type type)
    {
        State state = GetRegisteredState(type);
        if (state == null)
        {
            LogStateStatus("Not registered", type.ToString());
        }

        if (IsStacked(state) || nowTransitioning)
        {
            LogStateStatus("IsStacked(state) : ", IsStacked(state).ToString());
            LogStateStatus("nowTransitioning : ", nowTransitioning.ToString());
        }
        return state;
    }
    #endregion

    #region Initialize
    IEnumerator ProcessInitializeStates()
    {
        _stateManagerStatus = StateManagerStatus.INITIALIZE;

        LogStateStatus("ProcessInitializeStates", "Start Initialize start");

        State startState = _states[0];
        startState.gameObject.SetActive(true);

        yield return StartCoroutine(CommonManager.Instance.Init());

        _stateManagerStatus = StateManagerStatus.READY;

        LogStateStatus("ProcessInitializeStates", "Start Initialize End");

        _coroutineStateTransition = StartCoroutine(ProcessOpenState<BaseStateData>(startState, false, null));

    } 
    #endregion

    #region Open State
    IEnumerator ProcessPreOpenHandle<T>(State state, bool isShowLoadingScreen, T args) where T : BaseStateData
    {
        Debug.Log("ProcessPreOpenHandle " + state.name);
        ActiveBlockScreen(true);

        if (isShowLoadingScreen)
        {
            yield return ProcessShowLoadingScreen();
        }
        else
        {
            if(state.StateType == StateType.SCENE && state.IsFadeInOut)
                yield return ShowFadeInOut(true);
        }

        _stackedStates.Push(state);
        yield return ProcessPreOpenState(state, args);
    }

    IEnumerator ProcessOpeningHandle<T>(State state, bool isShowLoadingScreen, T args) where T : BaseStateData
    {
        Debug.Log("ProcessOpeningHandle " + state.name);
        if (isShowLoadingScreen)
        {
            yield return ProcessHideLoadingScreen();
        }
        else
        {
            if (state.StateType == StateType.SCENE && state.IsFadeInOut)
                yield return ShowFadeInOut(false);
        }

        yield return ProcessOpeningState(state);
    }

    IEnumerator ProcessPostOpenHandle<T>(State state, bool isShowLoadingScreen, T args) where T : BaseStateData
    {
        Debug.Log("ProcessPostOpenHandle " + state.name);
        ActiveBlockScreen(false);

        _stateManagerStatus = StateManagerStatus.READY;

        yield return ProcessPostOpenState(state);

        ProcessPostClosedState();
    }

    IEnumerator ProcessOpenState<T>(State state, bool isShowLoadingScreen, T args) where T : BaseStateData
    {
        yield return ProcessPreOpenHandle(state, isShowLoadingScreen, args);

        yield return CloseOtherStates(state);

        yield return ProcessOpeningHandle(state, isShowLoadingScreen, args);

        yield return ProcessPostOpenHandle(state, isShowLoadingScreen, args);
    }

    IEnumerator CloseOtherStates(State state)
    {
        int count = _stackedStates.Count;
        Debug.Log("CloseOtherStates : " + state.name + " / count : " + count);
        for (int i = 0; i < count; i++)
        {
            if (_stackedStates.Count > 0)
            {
                State cState = _stackedStates.Peek();
                Debug.Log("cState : " + cState.name + "/ state : " + state.name);
                if (!cState.Equals(state))
                {
                    yield return ProcessCloseState(cState, false);
                }
                else
                {
                    _stackedStates.Pop();
                }
            }
            
        }
        _stackedStates.Push(state);
    }

    IEnumerator ProcessOpenPushState<T>(State state, bool isShowLoadingScreen, T args) where T : BaseStateData
    {

        if (topState != null)
        {
            ProcessPauseState(topState);
        }

        yield return ProcessPreOpenHandle(state, isShowLoadingScreen, args);

        yield return ProcessOpeningHandle(state, isShowLoadingScreen, args);
        yield return ProcessPostOpenHandle(state, isShowLoadingScreen, args);
    }
    #endregion

    #region Close State
    IEnumerator ProcessClosingState(State state, bool isShowLoadingScreen)
    {
        if (_stateManagerStatus == StateManagerStatus.CLOSE_STATE)
        {
            ActiveBlockScreen(true);
        }

        if (isShowLoadingScreen)
        {
            yield return ProcessShowLoadingScreen();
        }

        _stackedStates.Pop();

        yield return ProcessClosingState(state);
    }

    IEnumerator ProcessClosedState(State state, bool isShowLoadingScreen)
    {
        yield return ProcessClosedState(state);
        if (isShowLoadingScreen)
        {
            yield return ProcessHideLoadingScreen();
        }
        if (topState != null)
        {
            yield return ProcessResumeState(topState);
        }

        if (_stateManagerStatus == StateManagerStatus.CLOSE_STATE)
        {
            ActiveBlockScreen(false);
            _stateManagerStatus = StateManagerStatus.READY;
        }
    }

    IEnumerator ProcessCloseState(State state, bool isShowLoadingScreen)
    {

        if (_stackedStates.Peek().Equals(state))
        {
            yield return ProcessClosingState(state, isShowLoadingScreen);
            yield return ProcessClosedState(state, isShowLoadingScreen);
            ProcessPostClosedState();
        }
        else
        {
            LogStateStatus(state.stateName, "Not Peek State");
        }

        
    }
    #endregion

    #region LoadingScreen
    IEnumerator ProcessShowLoadingScreen()
    {
        ActiveBlockScreen(true);

        _loadingScreen.OnShow(_loadingScreenAppearTime);

        yield return new WaitForSeconds(_loadingScreenAppearTime);

        ActiveBlockScreen(false);
    }

    IEnumerator ProcessHideLoadingScreen()
    {
        ActiveBlockScreen(true);

        yield return new WaitForSeconds(_loadingScreenMinMaintainTime);

        _loadingScreen.OnHide(_loadingScreenAppearTime);

        yield return new WaitForSeconds(_loadingScreenAppearTime);

        ActiveBlockScreen(false);
    }
    #endregion

    #region State Processes
    IEnumerator ProcessPreOpenState<T>(State state, T args) where T : BaseStateData
    {
        LogStateStatus(state.stateName, "PreOpen");

        state.transform.SetAsLastSibling();

        yield return new WaitForEndOfFrame();
        yield return state.OnPreOpen(args);

        state.gameObject.SetActive(true);
    }

    IEnumerator ProcessOpeningState(State state)
    {
        LogStateStatus(state.stateName, "Opening");

        yield return state.OnOpening();
    }

    IEnumerator ProcessPostOpenState(State state)
    {
        LogStateStatus(state.stateName, "PostOpen");

        yield return state.OnPostOpen();
    }

    IEnumerator ProcessClosingState(State state)
    {
        yield return state.OnClosing();

        LogStateStatus(state.stateName, "Closing");
    }

    IEnumerator ProcessClosedState(State state)
    {
        state.gameObject.SetActive(false);

        yield return state.OnClosed();

        LogStateStatus(state.stateName, "Closed");

        _unloadedStateList.Add(state);

    }

    private void ProcessPostClosedState()
    {
        for (int i = 0; i < _unloadedStateList.Count; i++)
        {
            LogStateStatus(_unloadedStateList[i].stateName, "PostClosed");
            _unloadedStateList[i].OnPostClosed();

            if (_unloadedStateList[i].IsDisposable)
            {
                _states.Remove(_unloadedStateList[i]);
                Destroy(_unloadedStateList[i].gameObject);
            }

        }
        _unloadedStateList.Clear();
    }

    void ProcessPauseState(State state)
    {
        LogStateStatus(state.stateName, "Pausing");

        state.OnPause();
    }

    IEnumerator ProcessResumeState(State state)
    {
        LogStateStatus(state.stateName, "Resuming");

        yield return state.OnResume();
    } 
    #endregion

    #region Spinner
    public void ShowSpinner()
    {
        _loadingSpinner.Show();
    }

    public void HideSpinner()
    {
        _loadingSpinner.Hide();
    }
    #endregion

    public IEnumerator ShowFadeInOut(bool isShow)
    {
        _fadeInOutCanvas.alpha = isShow ? 0f : 1f;
        _fadeInOutCanvas.LeanAlpha(isShow ? 1f : 0f, _fadeInOutTime);
        yield return new WaitForSeconds(_fadeInOutTime);
    }

    #region UTILTITES
    void LogStateStatus(string key, string status)
    {
        if (BuildManager.Instance.IsDevBuild())
        {
            _stringBuilder.Remove(0, _stringBuilder.Length);
            _stringBuilder.Append("<color=#FFFF00>");
            _stringBuilder.Append("[State Manager]");

            if (!string.IsNullOrEmpty(key))
            {
                _stringBuilder.AppendFormat("[{0}]", key);
            }

            _stringBuilder.AppendFormat(" {0}", status);
            _stringBuilder.Append("</color>");

            Debug.Log(_stringBuilder.ToString());
        }
    }
    #endregion UTILITIES

}
