using Common;
using __MyAssets._02.Scripts.VoiceChat.Remake;
using Cysharp.Threading.Tasks;
using FishNet.Transporting;
using UniRx;

///<summary>
///AirplaneMain 씬의 전체적인 흐름을 제어한다.
///StateMachine 기반으로 구현되어있다.
///</summary>
public class AppFlowPresenter : AppContext<AppFlowPresenter>
{
    #region Unity Lifecycle

    protected override void Awake()
    {
        base.Awake();
        
        if(Instance != this) return;
        
        SetAppFlow();
    }

    void Start() => AppFlowModel.Instance.AppFlow.Start(Define.AppFlowState.ParseXML);

    #endregion

    #region Private Methods

    void SetAppFlow()
    {
        var appFlow = AppFlowModel.Instance.AppFlow;

        //XML 파싱 진행
        //파싱 내용으로는 PlayerConfig, NetworkConfig, PositionConfig가 있음
        //PlayerConfig : Player Role 정보 및 VR 모드인지 여부 등
        //NetworkConfig : 네트워크 IP 및 포트 정보 등
        //PositionConfig : 씬 별로 초기 시작 위치
        appFlow.OnBeginByState(Define.AppFlowState.ParseXML).Subscribe(_ =>
        {
            Logger.Log("AppFlowState.ParseXML");
            //XML 파싱 작업 처리
            //XML 파싱 후 Setting에 할당
            ConfigModel.Instance.Setting = XMLManager.Instance.GetPlayerConfig();
            NetworkModel.Instance.Settings = XMLManager.Instance.GetNetworkConfig();
            PositionModel.Instance.Settings = XMLManager.Instance.GetPositionConfig();

            appFlow.Transition(Define.AppFlowState.Setting);
        }).AddTo(gameObject);

        //시작 전 초기 세팅 진행
        //PlayerConfig 정보를 기반으로 Player의 Role을 결정
        appFlow.OnBeginByState(Define.AppFlowState.Setting).Subscribe(async _ =>
        {
            Logger.Log("AppFlowState.Setting");
            NetworkSystem.Instance.Connect();

            NetworkEventModel.Instance.SetNetworkEvent();

            if(!ConfigModel.Instance.Setting.debugMode)
            {
                VoiceChatManager.Instance.Initialize(ConfigModel.Instance.Setting.observerIP, DataModel.Instance.IsObserver);
                VoiceChatManager.Instance.ConnectionState.FirstOrDefault(state => state == LocalConnectionState.Started).ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy()).Forget();
            }
            
            appFlow.Transition(Define.AppFlowState.Main);
        }).AddTo(gameObject);

        //실제로 작업하게 되면 Main 내부적으로도 SubState가 세분화 되리라 예상
        appFlow.OnBeginByState(Define.AppFlowState.Main).Subscribe(_ =>
        {
            Logger.Log("AppFlowState.Main");

            if(!ConfigModel.Instance.Setting.debugMode) ObserverLobbySystem.Instance.ShowObserverLobby();
            DataModel.Instance.USModels.OnlyConnected().ForEach(us => SpawnPlayer.Instance.SpawnPlayerOnPoint(us.Player));
        }).AddTo(gameObject);
    }

    #endregion
}