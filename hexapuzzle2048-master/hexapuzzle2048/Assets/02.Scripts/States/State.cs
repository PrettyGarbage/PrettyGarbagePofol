using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class State : MonoBehaviour
{
    public static StringBuilder stringBuilder = new StringBuilder();

    [SerializeField] Canvas _UI;

    Dictionary<string, object> _args;

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
    public virtual IEnumerator OnLoad(Dictionary<string, object> args = null)
    {
        if (_UI != null)
        {
            _UI.enabled = true;
        }

        _args = args;

        yield return null;
    }

    /// <summary>
    /// Called after the loading screen disappears.
    /// </summary>
    public virtual IEnumerator OnEnter()
    {
        yield return null;
    }

    public virtual IEnumerator OnEntered()
    {
        yield return null;
    }

    /// <summary>
    /// Called when a state is popped from the stack.
    /// Not called during stack initialization When called by OpenState().
    /// </summary>
    public virtual IEnumerator OnExit()
    {
        yield return null;
    }

    /// <summary>
    /// Called when a state is popped after OnExit() or remove from the stack.
    /// </summary>
    public virtual IEnumerator OnUnload()
    {
        if (_UI != null)
        {
            _UI.enabled = false;
        }

        _args = null;

        StopAllCoroutines();

        yield return null;
    }

    /// <summary>
    /// Called when in top state in the state stack per frame.
    /// </summary>
    public virtual void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnBack();
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
        StateManager.instance.PopState(stateName);
    }

    public virtual void OnAppPause()
    {
    }

    public virtual void OnAppResume()
    {
    }

    public virtual string stateName
    {
        get
        {
            return "";
        }
    }

    public Canvas UI
    {
        get
        {
            return _UI;
        }
    }

    public Dictionary<string, object> args
    {
        get
        {
            return _args;
        }
    }

    public bool isStacked
    {
        get
        {
            return StateManager.instance.IsStacked(stateName);
        }
    }

    public bool isTopped
    {
        get
        {
            return (StateManager.instance.topState == this);
        }
    }

    public bool isPaused
    {
        get
        {
            return (StateManager.instance.IsStacked(stateName) &&
                    StateManager.instance.topState != this);
        }
    }

    #region UTILITIES
    public T GetParam<T>(string key, T defaultValue)
    {
        if (_args == null ||
            !_args.ContainsKey(key))
        {
            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.AppendFormat(
                "Not contained {0}({1}) key in {2} state", key, typeof(T), stateName
            );
            Debug.LogWarning(stringBuilder.ToString());

            return defaultValue;
        }

        object value = _args[key];
        if (value == null)
        {
            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.AppendFormat(
                "Found {0}({1}) key in {2} state, But not contained any value", key, typeof(T), stateName
            );

            return defaultValue;
        }

        if (value.GetType() != typeof(T))
        {
            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.AppendFormat(
                "a parameter {0}'s type mismatch in {1} state", key, stateName
            );
            stringBuilder.AppendFormat(
                "(Stored: {0} / Requested: {1})", value.GetType(), typeof(T)
            );
            Debug.LogWarning(stringBuilder.ToString());

            return defaultValue;
        }

        return (T)value;
    }

    public void SetParam(string key, object value)
    {
        if (_args == null)
        {
            _args = new Dictionary<string, object>();
        }

        if (_args.ContainsKey(key))
        {
            _args[key] = value;
        }
        else
        {
            _args.Add(key, value);
        }
    }
    #endregion UTILITITES
}