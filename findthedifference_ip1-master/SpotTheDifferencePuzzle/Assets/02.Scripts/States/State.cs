using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStateData
{
    
}

public class State : MonoBehaviour
{
    public static StringBuilder stringBuilder = new StringBuilder();

    [SerializeField]
    protected StateType _stateType = StateType.SCENE;
    [SerializeField]
    protected bool _isFadeInOut = false;
    [SerializeField]
    protected bool _isDisposable = false;

    public StateType StateType { get { return _stateType; } }
    public bool IsFadeInOut { get { return _isFadeInOut; } }
    public bool IsDisposable { get { return _isDisposable; } }
    public string stateName {  get { return this.GetType().ToString(); }  }
    public BaseStateData _stateData;

    /// <summary>
    /// Initialized state. Called only once at application start.
    /// </summary>
    public virtual IEnumerator OnInitialize()
    {
        yield return null;
    }

    /// <summary>
    /// Called when a state is loaded with the loading screen appears.
    /// </summary>
    /// <param name="args">State changeable parameters.</param>
    public virtual IEnumerator OnPreOpen<T>(T args = default(T)) where T : BaseStateData
    {
        _stateData = args;
        yield return null;
    }

    /// <summary>
    /// Called after the loading screen disappears.
    /// </summary>
    public virtual IEnumerator OnOpening()
    {
        yield return null;
    }

    public virtual IEnumerator OnPostOpen()
    {
        yield return null;
    }

    /// <summary>
    /// Called when a state is popped from the stack.
    /// Not called during stack initialization When called by OpenState().
    /// </summary>
    public virtual IEnumerator OnClosing()
    {
        yield return null;
    }

    /// <summary>
    /// Called when a state is popped after OnExit() or remove from the stack.
    /// </summary>
    public virtual IEnumerator OnClosed()
    {
        _stateData = null;

        StopAllCoroutines();

        yield return null;
    }

    /// <summary>
    /// 화면전환이 완전히 끝난 후에.
    /// </summary>
    public virtual void OnPostClosed()
    {

    }

    /// <summary>
    /// Called when in top state in the state stack per frame.
    /// </summary>
    public virtual void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!StateManager.Instance.IsBlockScreen)
            {
                OnBack();
            }
        }
    }

    public virtual void OnPause()
    {
    }

    public virtual IEnumerator OnResume()
    {
        yield return null;
    }

    public virtual void OnBack()
    {
        
    }

    public virtual void OnAppPause()
    {
    }

    public virtual void OnAppResume()
    {
    }
    
    public bool isStacked
    {
        get
        {
            return StateManager.Instance.IsStacked(stateName);
        }
    }

    public bool isTopped
    {
        get
        {
            return (StateManager.Instance.topState == this);
        }
    }

    public bool isPaused
    {
        get
        {
            return (StateManager.Instance.IsStacked(stateName) &&
                    StateManager.Instance.topState != this);
        }
    }

    public T GetData<T>() where T : BaseStateData, new()
    {
        return _stateData!=null ? (T)_stateData : new T();
    }

}