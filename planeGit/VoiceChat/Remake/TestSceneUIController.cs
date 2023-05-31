using __MyAssets._02.Scripts.VoiceChat.Remake;
using FishNet.Transporting;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// 테스트용이라서 대충 만든거라 설계가 없습니다.
/// </summary>
public class TestSceneUIController : MonoBehaviour
{
    [SerializeField] private TMP_InputField ipAddressInputField;
    [SerializeField] private Toggle isHostToggle;
    [SerializeField] private Button connectButton;
    [SerializeField] private TextMeshProUGUI connectionStateText;
    [SerializeField] private VoiceChatManager voiceChatManager;
    
    private bool _isConnected = false;
    // Start is called before the first frame update
    void Start()
    {
        connectButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                if (_isConnected)
                {
                    voiceChatManager.Disconnect();
                }
                else
                {
                    var ip = ipAddressInputField.text;
                    var isHost = isHostToggle.isOn;
                    voiceChatManager.Initialize(ip, isHost);
                }
            });

        voiceChatManager.ConnectionState.Subscribe(str =>
        {
            connectionStateText.text = str switch
            {
                LocalConnectionState.Started => "Connected",
                LocalConnectionState.Stopped => "Disconnected",
                _ => str.ToString()
            };

            _isConnected = str == LocalConnectionState.Started;
        });
    }
    
}
