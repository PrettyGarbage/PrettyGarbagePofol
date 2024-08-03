using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PauseState : PopupState
{
    [Header("Pause")]
    [SerializeField] GameObject _cheatRoot;

    public override IEnumerator OnInitialize()
    {
        yield return base.OnInitialize();

        bool activeCheat = 
            BuildManager.Instance.IsDevBuild() &&
            Application.platform != RuntimePlatform.Android &&
            Application.platform != RuntimePlatform.IPhonePlayer;

        _cheatRoot.SetActive(activeCheat);

        yield return null;
    }


    #region EVENTS
    public void OnGoThemeList()
    {
        StateManager.Instance.OpenState<BaseStateData>(typeof(SelectThemeState), true);
    }

    public void OnGoReady()
    {
        StateManager.Instance.OpenState<BaseStateData>(typeof(PopupReadyState), true);
    }

    public void OnGiveUp()
    {
        InGameState state = StateManager.Instance.GetRegisteredState(typeof(InGameState)) as InGameState;
        if (state != null)
        {
            state.OnGiveUp();
        }

        OnBack();
    }

    public void OnAutoCompleteRound()
    {
        if (!BuildManager.Instance.IsDevBuild())
        {
            return;
        }

        InGameState state = StateManager.Instance.GetRegisteredState(typeof(InGameState)) as InGameState;
        if (state != null)
        {
            state.OnAutoCompleteRound();
        }

        OnBack();
    }

    public void OnAutoFailureRound()
    {
        if (!BuildManager.Instance.IsDevBuild())
        {
            return;
        }

        InGameState state = StateManager.Instance.GetRegisteredState(typeof(InGameState)) as InGameState;
        if (state != null)
        {
            state.OnAutoFailureRound();
        }

        OnBack();
    }

    public void OnTestOption()
    {
        if (!BuildManager.Instance.IsDevBuild())
        {
            return;
        }

        InGameState state = StateManager.Instance.GetRegisteredState(typeof(InGameState)) as InGameState;
        if (state != null)
        {
            state.SetTestMode();
        }
    }
    #endregion EVENTS
}
