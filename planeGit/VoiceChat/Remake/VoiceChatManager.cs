using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Cysharp.Threading.Tasks;
using Dissonance;
using FishNet;
using FishNet.Managing;
using FishNet.Transporting;
using FishNet.Transporting.Tugboat;
using UniRx;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace __MyAssets._02.Scripts.VoiceChat.Remake
{
    public class VoiceChatManager : AppContext<VoiceChatManager>
    {
        #region Variables

        NetworkManager _networkManager;
        Tugboat _tugboat;
        bool _startingAsHost;
        private string _ipAddress = string.Empty;

        #endregion

        #region Serialized Fields

        [SerializeField] DissonanceComms dissonanceSetup;
        [SerializeField] GameObject voiceChatTrigger;
        [SerializeField] int portNumber = 30300;

        #endregion

        #region Properties

        //친절하게 Observable로 Speaking OnStart와 OnStopped를 체크할 수 있는 Observable
        public IObservable<long> OnCheckVoiceMissionDeltaTime = null;
        public ReactiveProperty<LocalConnectionState> ConnectionState { get; private set; } = new();
        
        #endregion

        #region Unity LifeCycle

        protected override void Awake()
        {
            base.Awake();
            _networkManager = InstanceFinder.NetworkManager;
            _networkManager.ServerManager.OnServerConnectionState += ServerManager_OnServerConnectionState;
            _networkManager.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
            
            //Example
            // Initialize("192.168.0.192", true);
        }

        private void OnDestroy()
        {
            if(_networkManager == null) return;
            _networkManager.ServerManager.OnServerConnectionState -= ServerManager_OnServerConnectionState;
            _networkManager.ClientManager.OnClientConnectionState -= ClientManager_OnClientConnectionState;

            if(_startingAsHost)
                _networkManager.ServerManager.StopConnection(true);
            _networkManager.ClientManager.StopConnection();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Host인지 Client인지 분기해서 초기화
        /// </summary>
        /// <param name="ipAddress">클라이언트 엔드포인트</param>
        /// <param name="isHost">true면 Host 접속 false면 클라이언트 접속</param>
        public void Initialize(string ipAddress, bool isHost = false)
        {
            _startingAsHost = false;

            _tugboat = GetComponent<Tugboat>();
            _tugboat.SetClientAddress(ipAddress);
            _tugboat.SetPort((ushort)portNumber);
            
            if (isHost)
            {
                _networkManager.ServerManager.StartConnection();
                Debug.LogError(_networkManager);
                Debug.LogError(_networkManager.ServerManager);
            }
            
            _networkManager.ClientManager.StartConnection();
            _ipAddress = ipAddress;
            WaitForConnection().Forget();
        }
        
        public void Disconnect()
        {
            if(_startingAsHost)
                _networkManager.ServerManager.StopConnection(true);
            _networkManager.ClientManager.StopConnection();
        }

        #endregion

        #region Private Methods

        private void ServerManager_OnServerConnectionState(ServerConnectionStateArgs obj)
        {
            Logger.Log("ServerManager_OnServerConnectionState :: " + obj.ConnectionState, Logger.LogLevel.Network);
        }
        
        private void ClientManager_OnClientConnectionState(ClientConnectionStateArgs obj)
        {
            Logger.Log("ClientManager_OnClientConnectionState :: " + obj.ConnectionState, Logger.LogLevel.Network);
            
            ConnectionState.Value = obj.ConnectionState;
            
            if(obj.ConnectionState == LocalConnectionState.Stopped)
                Initialize(_ipAddress);
        }

        /// <summary>
        /// 네트워크와 Dissonance가 초기화 될 때까지 대기
        /// </summary>
        async UniTaskVoid WaitForConnection()
        {
            await UniTask.WaitUntil(() => _networkManager.Initialized && !_networkManager.IsOffline);

            dissonanceSetup.gameObject.SetActive(true);

            await UniTask.WaitUntil(() => dissonanceSetup.GetComponent<DissonanceComms>().IsNetworkInitialized);
            voiceChatTrigger.SetActive(true);
            
            VoiceChatMissionStreamSetting();
        }
        
        /// <summary>
        /// 보이스 채팅 미션 스트림 IObservable로 제공 구독해서 갖고간 뒤에 Started -> Stopped될 때
        /// 시간을 체크할 수 있도록 한다.
        /// </summary>
        private void VoiceChatMissionStreamSetting()
        {
            // OnStartedSpeaking 이벤트 구독
            var startedSpeaking = Observable.FromEvent<VoicePlayerState>(
                    h => dissonanceSetup.Players[0].OnStartedSpeaking += h,
                    h => dissonanceSetup.Players[0].OnStartedSpeaking -= h)
                .Where(_ => dissonanceSetup && dissonanceSetup.Players.Count > 0)
                .TakeUntilDestroy(gameObject);

            // OnStoppedSpeaking 이벤트 구독
            var stoppedSpeaking = Observable.FromEvent<VoicePlayerState>(
                    h => dissonanceSetup.Players[0].OnStoppedSpeaking += h,
                    h => dissonanceSetup.Players[0].OnStoppedSpeaking -= h)
                .Where(_ => dissonanceSetup && dissonanceSetup.Players.Count > 0)
                .TakeUntilDestroy(gameObject);

            OnCheckVoiceMissionDeltaTime = Observable.EveryUpdate()
                .SkipUntil(startedSpeaking)
                .TakeUntil(stoppedSpeaking)
                .RepeatUntilDestroy(gameObject);
        }

        #endregion
    }
}