using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;

public class ObserverLobbyItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI statusText;
    [SerializeField] Transform circleConnect;
    [SerializeField] Transform circleDisconnect;
    public void InitLobbyItem(UserStateModel model)
    {
        nameText.text = $"Player{(int)model.Role.Value}";

        if (DataModel.Instance.MyId == (int)model.Role.Value) nameText.color = Color.yellow;

        model.Status.Subscribe(value => statusText.text = value.ToString());
        model.IsConnected.Subscribe(value =>
        {
            if (value)
            {
                circleConnect.gameObject.SetActive(true);
                circleDisconnect.gameObject.SetActive(false);
            }
            else
            {
                circleConnect.gameObject.SetActive(false);
                circleDisconnect.gameObject.SetActive(true);
            }
        });
        
        gameObject.SetActive(true);
    }
}
