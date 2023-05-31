using System.Collections;
using System.Collections.Generic;
using Common;
using UniRx;
using UnityEngine;

public class ObserverLobbySystem : SceneContext<ObserverLobbySystem>
{
    private ObserverLobbyPresenter _lobbyPresenter;
    public void ShowObserverLobby()
    {
        _lobbyPresenter = Managers.Resource.Instantiate(Constants.PrefabUI("ObserverLobbyCanvas"), null).GetComponent<ObserverLobbyPresenter>();
    }

    public void HideObserverLobby()
    {
        if (!_lobbyPresenter)
            return;
        Managers.Resource.Destroy(_lobbyPresenter.gameObject);
    }
}
