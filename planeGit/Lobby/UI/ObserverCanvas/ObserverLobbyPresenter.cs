using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class ObserverLobbyPresenter : MonoBehaviour
{
    [SerializeField] private GameObject _item;

    public void Awake()
    {
        SpawnItems();
    }

    private void SpawnItems()
    {
        for (int index = 1; index < 5; index++)
        {
            var lobbyItem = Instantiate(_item, _item.transform.parent).GetComponent<ObserverLobbyItem>();

            UserStateModel model = DataModel.Instance.USModels.FirstOrDefault(us => (int)us.Role.Value == index);
            lobbyItem.InitLobbyItem(model);
        }
    }
}