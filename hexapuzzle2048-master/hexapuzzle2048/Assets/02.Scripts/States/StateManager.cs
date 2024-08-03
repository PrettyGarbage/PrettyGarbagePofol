using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class StateManager : MonoBehaviour
{
    #region SINGLETON
    static StateManager _instance;

    public static StateManager instance
    {
        get
        {
            return _instance;
        }
    }
    #endregion SINGLETON

    [SerializeField, HideInInspector] string _startStateName;
    [SerializeField] State[] _states;

    [Header("Loading Screen")]
    [SerializeField] LoadingScreen _loadingScreen;
    [SerializeField] float _loadingScreenAppearTime = 1.0f;
    [SerializeField] float _loadingScreenMinMaintainTime = 0.5f;

    [Header("Block Screen")]
    [SerializeField] RectTransform _blockScreen;

    Stack<State> _stackedStates = new Stack<State>();
    State _lastState;

    Coroutine _coroutineStateTransition;

    int _blockScreenActiveCount = 0;

    StringBuilder _stringBuilder = new StringBuilder();

    #region UNITY EVENTS
    private void Awake()
    {
        #region SINGLETON
        if (_instance == null)
        {
            _instance = this;

            DontDestroyOnLoad(gameObject);

            if (_loadingScreen != null)
            {
                _loadingScreen.gameObject.SetActive(true);
                _loadingScreen.OnInitialize();
            }
        }
        else if (_instance != this)
        {
            Destroy(this);
        }
        #endregion SINGLETON
    }

    private IEnumerator Start()
    {
        _states = Array.FindAll(_states, s => s != null);
        if (_states.Length <= 0)
        {
            LogStateStatus("ERROR", "Empty states");

            yield break;
        }

        for (int i = 0, max = _states.Length; i < max; ++i)
        {
            _states[i].gameObject.SetActive(true);
            _states[i].enabled = false;
            
            if (_states[i].UI != null)
            {
                _states[i].UI.enabled = false;
            }
        }

        State startState = GetRegisteredState(_startStateName);

        ActiveBlockScreen(true);

        yield return ProcessInitializeStates(startState);

        _stackedStates.Push(startState);

        yield return ProcessLoadState(startState, null);
        yield return ProcessHideLoadingScreen();
        yield return ProcessEnterState(startState);

        ActiveBlockScreen(false);
        yield return ProcessEnteredState(startState);
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

    public bool OpenState(State state, Dictionary<string, object> args = null)
    {
        if (state == null || IsStacked(state) || nowTransitioning)
        {
            return false;
        }

        _coroutineStateTransition =
            StartCoroutine(ProcessOpenState(state, args));

        return true;
    }

    public bool OpenState(string stateName, Dictionary<string, object> args = null)
    {
        State state = GetRegisteredState(stateName);
        if (state == null)
        {
            LogStateStatus("Not registered", stateName);
        }

        return OpenState(state, args);
    }

    public bool OpenStateForced(State state, Dictionary<string, object> args = null)
    {
        if (state == null || nowTransitioning)
        {
            return false;
        }

        _coroutineStateTransition =
            StartCoroutine(ProcessOpenState(state, args));

        return true;
    }
    
    public bool OpenStateForced(string stateName, Dictionary<string, object> args = null)
    {
        State state = GetRegisteredState(stateName);
        if (state == null)
        {
            LogStateStatus("Not registered", stateName);
        }

        return OpenStateForced(GetRegisteredState(stateName), args);
    }

    public bool PushState(State state, Dictionary<string, object> args = null)
    {
        if (state == null || nowTransitioning)
        {
            return false;
        }

        _coroutineStateTransition =
            StartCoroutine(ProcessPushState(state, args));

        return true;
    }

    public bool PushState(string stateName, Dictionary<string, object> args = null)
    {
        return PushState(GetRegisteredState(stateName), args);
    }

    public bool PushStateForced(State state, Dictionary<string, object> args = null)
    {
        if (state == null || nowTransitioning)
        {
            return false;
        }

        _coroutineStateTransition =
            StartCoroutine(ProcessPushStateForced(state, args));

        return true;
    }

    public bool PushStateForced(string stateName, Dictionary<string, object> args = null)
    {
        return PushStateForced(GetRegisteredState(stateName), args);
    }

    public bool PopState(bool showLoadingScreen = false)
    {
        if (nowTransitioning || _stackedStates.Count <= 1)
        {
            return false;
        }

        _coroutineStateTransition =
            StartCoroutine(ProcessPopState(showLoadingScreen));

        return true;
    }

    public bool PopState(string stateName, bool showLoadingScreen = false)
    {
        if (nowTransitioning)
        {
            return false;
        }

        if (string.IsNullOrEmpty(stateName) || !IsStacked(stateName))
        {
            return false;
        }

        _coroutineStateTransition =
            StartCoroutine(ProcessPopState(stateName, showLoadingScreen));

        return true;
    }

    public bool PopState(State state, bool showLoadingScreen = false)
    {
        if (state == null)
        {
            return false;
        }

        return PopState(state.stateName, showLoadingScreen);
    }

    public void ActiveBlockScreen(bool active)
    {
        if (_blockScreen == null)
        {
            return;
        }

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

    public State GetRegisteredState(string stateName)
    {
        if (string.IsNullOrEmpty(stateName) || _states.Length <= 0)
        {
            return null;
        }

        for (int i = 0, max = _states.Length; i < max; ++i)
        {
            if (string.Compare(_states[i].stateName, stateName, true) == 0)
            {
                return _states[i];
            }
        }

        return null;
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

    public string[] stateNames
    {
        get
        {
            List<string> names = new List<string>();
            if (_states != null)
            {
                for (int i = 0, max = _states.Length; i < max; ++i)
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

    public bool nowTransitioning
    {
        get
        {
            return (_coroutineStateTransition != null);
        }
    }

    public State topState
    {
        get
        {
            return (_stackedStates.Count > 0) ? _stackedStates.Peek() : null;
        }
    }

    public State LastState
    {
        get
        {
            return _lastState;
        }
    }

    #region COROUTINES
    IEnumerator ProcessInitializeStates(State startState)
    {
        if (startState == null)
        {
            startState = _states[0];
        }

        startState.enabled = true;
        yield return startState.OnInitialize();

        LogStateStatus(startState.stateName, "Start state Initialize complete");

        yield return StartCoroutine(CommonManager.Instance.Init());

        for (int i = 0, max = _states.Length; i < max; ++i)
        {
            if (_states[i] != startState)
            {
                yield return _states[i].OnInitialize();

                LogStateStatus(_states[i].stateName, "Initialize complete");
            }
        }
    }

    IEnumerator ProcessOpenState(State state, Dictionary<string, object> args)
    {
        if (state == null)
        {
            _coroutineStateTransition = null;

            yield break;
        }

        ActiveBlockScreen(true);

        yield return ProcessShowLoadingScreen();

        while (_stackedStates.Count > 0)
        {
            yield return ProcessUnloadState(_stackedStates.Pop());
        }

        _lastState = topState;
        _stackedStates.Push(state);

        yield return ProcessLoadState(state, args);
        yield return ProcessHideLoadingScreen();
        yield return ProcessEnterState(state);
        
        ActiveBlockScreen(false);

        _coroutineStateTransition = null;
        yield return ProcessEnteredState(state);
    }

    IEnumerator ProcessPushState(State state, Dictionary<string, object> args)
    {
        if (state == null || IsStacked(state))
        {
            _coroutineStateTransition = null;

            yield break;
        }

        ActiveBlockScreen(true);

        if (topState != null)
        {
            ProcessPauseState(topState);
        }
        
        _lastState = topState;
        _stackedStates.Push(state);

        yield return ProcessLoadState(state, args);
        yield return ProcessEnterState(state);

        ActiveBlockScreen(false);

        _coroutineStateTransition = null;
        yield return ProcessEnteredState(state);
    }

    IEnumerator ProcessPushStateForced(State state, Dictionary<string, object> args)
    {
        if (state == null)
        {
            _coroutineStateTransition = null;

            yield break;
        }

        ActiveBlockScreen(true);

        yield return ProcessShowLoadingScreen();

        if (IsStacked(state))
        {
            while (string.Compare(topState.stateName, state.stateName, true) != 0)
            {
                yield return ProcessUnloadState(_stackedStates.Pop());
            }

            yield return ProcessUnloadState(_stackedStates.Pop());
        }

        _stackedStates.Push(state);

        yield return ProcessLoadState(state, args);
        yield return ProcessHideLoadingScreen();
        yield return ProcessEnterState(state);

        ActiveBlockScreen(false);

        _coroutineStateTransition = null;
        yield return ProcessEnteredState(state);
    }

    IEnumerator ProcessPopState(bool showLoadingScreen)
    {
        if (topState == null)
        {
            _coroutineStateTransition = null;

            yield break;
        }

        yield return ProcessPopState(topState.stateName, showLoadingScreen);
    }

    IEnumerator ProcessPopState(string stateName, bool showLoadingScreen)
    {
        if (topState == null)
        {
            _coroutineStateTransition = null;

            yield break;
        }

        ActiveBlockScreen(true);

        if (showLoadingScreen)
        {
            yield return ProcessShowLoadingScreen();
        }

        while (string.Compare(topState.stateName, stateName, true) != 0)
        {
            yield return ProcessExitState(_stackedStates.Peek());
            yield return ProcessUnloadState(_stackedStates.Pop());
        }

        yield return ProcessExitState(_stackedStates.Peek());
        yield return ProcessUnloadState(_stackedStates.Pop());

        if (showLoadingScreen)
        {
            yield return ProcessHideLoadingScreen();
        }

        if (topState != null)
        {
            yield return ProcessResumeState(topState);
        }

        ActiveBlockScreen(false);

        _coroutineStateTransition = null;
    }

    IEnumerator ProcessShowLoadingScreen()
    {
        if (_loadingScreen == null)
        {
            yield break;
        }

        ActiveBlockScreen(true);

        _loadingScreen.OnShow(_loadingScreenAppearTime, 0.1f);

        yield return new WaitForSeconds(_loadingScreenAppearTime + 0.1f);

        ActiveBlockScreen(false);
    }

    IEnumerator ProcessHideLoadingScreen()
    {
        if (_loadingScreen == null)
        {
            yield break;
        }

        ActiveBlockScreen(true);

        yield return new WaitForSeconds(_loadingScreenMinMaintainTime);

        _loadingScreen.OnHide(_loadingScreenAppearTime, 0.0f);

        yield return new WaitForSeconds(_loadingScreenAppearTime);

        ActiveBlockScreen(false);
    }

    IEnumerator ProcessLoadState(State state, Dictionary<string, object> args)
    {
        LogStateStatus(state.stateName, "Loading");

        state.transform.SetAsLastSibling();

        yield return new WaitForEndOfFrame();
        yield return state.OnLoad(args);

        state.enabled = true;
    }

    IEnumerator ProcessEnterState(State state)
    {
        LogStateStatus(state.stateName, "Entering");

        yield return state.OnEnter();
    }

    IEnumerator ProcessEnteredState(State state)
    {
        LogStateStatus(state.stateName, "Entered");

        yield return state.OnEntered();
    }

    IEnumerator ProcessExitState(State state)
    {
        yield return state.OnExit();

        LogStateStatus(state.stateName, "Exited");
    }

    IEnumerator ProcessUnloadState(State state)
    {
        state.enabled = false;

        yield return state.OnUnload();

        LogStateStatus(state.stateName, "Unloaded");
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
    #endregion COROUTINES

    #region UTILTITES
    void LogStateStatus(string key, string status)
    {
#if DEBUG
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
#endif
    }
    #endregion UTILITIES
}