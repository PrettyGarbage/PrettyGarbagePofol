using System.Threading.Tasks;
using Common;
using Cysharp.Threading.Tasks;
using Framework.Common.Template.SceneLoader;
using TMPro;
using UnityEngine;
using Valve.VR.InteractionSystem;
using UniRx;
using CoolishUI;

public class ScenarioSystem : SceneContext<ScenarioSystem>
{
    #region Properties

    public Define.ScenarioMode mode;
    public int targetEventIndex;
    public int targetMissionIndex;
    
    [field: SerializeField] public Scenario CurrentScenario { get; protected set; }
    public int testNo = 0;

    public float timeSpeed = 1f;

    #endregion

    #region Public Methods

    public void ScenarioStart()
    {
        Initialize();
        CurrentScenario.StartMission();
        string userId = DataModel.Instance.Mine.Role.Value switch
        {
            Define.Role.CCS => "Observer",
            Define.Role.CC1 => DataModel.Instance.OPSModel.Crew1UserID.Value,
            Define.Role.CC2 => DataModel.Instance.OPSModel.Crew2UserID.Value,
            Define.Role.CC3 => DataModel.Instance.OPSModel.Crew3UserID.Value,
            Define.Role.CC4 => DataModel.Instance.OPSModel.Crew4UserID.Value,
            _ => "Unknown"
        };
        int deviceIndex = DataModel.Instance.MyId;
        DataModel.Instance.Mine.Player.GetVideoCapture().StartCapture(
            DataModel.Instance.OPSModel.TrainingCode.Value, 
            $"Replay_{DataModel.Instance.Mine.Role.Value}_{userId}_{deviceIndex}"
            );
    }

    public void NextScenarioEvent()
    {
        CurrentScenario.Next();
    }
    
    public async UniTask TrainingFinish()
    {
        DataModel.Instance.Mine.Status.Value = Define.Status.TrainingFinish;
        await GoToLobby();
    }
    
    public async UniTask TrainingStop()
    {
        DataModel.Instance.Mine.Status.Value = Define.Status.Stop;
        await GoToLobby();
    }

    static async Task GoToLobby()
    {
        foreach (var hand in Valve.VR.InteractionSystem.Player.instance.hands)
        {
            for (int i = hand.AttachedObjects.Count - 1; i >= 0; i--)
            {
                AttachSystem.Instance.Detach(hand.AttachedObjects[i].attachedObject.GetComponent<Interactable>(), hand);
            }
        }

        RaycastSystem.Instance.EndRaycast();
        
        DataModel.Instance.Mine.Player.GetVideoCapture().EndCapture();
        await SceneLoader.Instance.LoadSceneAsync(0);

        var xrCameraCanvas = DataModel.Instance.Mine.Player.GetComponentInChildren<XRCameraCanvas>(true);
        xrCameraCanvas.gameObject.SetActive(false);

        DataModel.Instance.USModels.Mine().Reconnect();
        ObserverLobbySystem.Instance.ShowObserverLobby();
    }

    void Initialize()
    {
        CurrentScenario.Initialize();
        var xrCameraCanvas = DataModel.Instance.Mine.Player.GetComponentInChildren<XRCameraCanvas>(true);
        xrCameraCanvas.gameObject.SetActive(mode == Define.ScenarioMode.XR);
        Valve.VR.InteractionSystem.Player.instance.hands.ForEach(hand => hand.SetVisibility(mode != Define.ScenarioMode.XR));
        if (DataModel.Instance.IsObserver && mode != Define.ScenarioMode.VR)
        {
            var canvas = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Canvas - Observer"));
            canvas.GetComponentInChildren<TextMeshProUGUI>().text = mode switch
            {
                Define.ScenarioMode.XR => "XR 모드 진행 중...",
                Define.ScenarioMode.Single => "Single 모드 진행 중...",
                _ => "Unknown"
            };
        }
    }

    #endregion

    #region Unity LifeCycle

    async void Start()
    {
        await UniTask.Delay(1000);
        if (ConfigModel.Instance.Setting.debugMode)
        {
            Logger.Log("Initialize");
            Initialize();
            RaycastSystem.Instance.StartRaycast();
            CurrentScenario.StartMission();

            // GetSubscribeData.Parse();
        }
    }
    
    void Update()
    {
        if(ConfigModel.Instance.Setting.debugMode) Time.timeScale = timeSpeed;
    }
    
    #endregion
}