using System.Linq;
using Common;
using CoolishUI;
using UniRx;
using UnityEngine;

public class ScenarioEvent : MonoBehaviour
{
    #region Fields

    ScenarioEventProduction production;

    #endregion

    #region Properties

    Scenario MyScenario { get; set; }
    public Mission[] Missions { get; } = new Mission[4];
    Mission MyMission => ConfigModel.Instance.Setting.role != 0 ? Missions[ConfigModel.Instance.Setting.role - 1] : null;
    public string EventCode => gameObject.name;
    public ScenarioEventProduction Production { get => production; }

    #endregion

    #region Unity Lifecycle

    void Awake() => production = GetComponent<ScenarioEventProduction>();

    #endregion

    #region Public Methods

    public void Initialize(Scenario scenario)
    {
        MyScenario = scenario;
        if (production != null) production.EventCode = EventCode;

        var crewMissions = GetComponents<Mission>();
        Missions[0] = crewMissions.FirstOrDefault(mission => mission.TargetCrew == Define.CrewType.CrewA);
        Missions[1] = crewMissions.FirstOrDefault(mission => mission.TargetCrew == Define.CrewType.CrewB);
        Missions[2] = crewMissions.FirstOrDefault(mission => mission.TargetCrew == Define.CrewType.CrewC);
        Missions[3] = crewMissions.FirstOrDefault(mission => mission.TargetCrew == Define.CrewType.CrewD);

        for (int i = 0; i < Missions.Length; i++)
        {
            Missions[i] = Missions[i] ? Missions[i] : gameObject.AddComponent<MissionSkip>();
        }

        Missions.ForEach((mission, role) => mission.Initialize(EventCode, role + 1));
    }

    public async void OnBegin()
    {
        Logger.Log($"OnBegin {EventCode}");
        SimpleDebugConsole.Instance.AddLog($"{EventCode} 시작");

        DataModel.Instance.USModels.OnlyClient().OnlyConnected().ForEach((us, role) =>
        {
            SimpleDebugConsole.Instance.AddLog($"CC{role}, {EventCode} 1번째 미션 시작");
            us.MissionCode.Where(code => code != 0).Where(_ => EventCode == us.EventCode.Value).Subscribe(_ => SimpleDebugConsole.Instance.AddLog($"CC{role}, {EventCode} {us.MissionCode.Value + 1}번째 미션 시작"));
        });

        if(DataModel.Instance.OPSModel.Scenario.Value != "TW") 
            DataModel.Instance.USModels.Mine().EventCode.Value = EventCode;

        DataModel.Instance.USModels.Mine().EventStatus.Value = Define.EventStatus.Playing;
        bool isObserver = ConfigModel.Instance.Setting.role == 0;
        if (production != null) await production.OnPrevStartMission(isObserver);
        Missions.ForEach(mission => mission.SetMission());
        if(ConfigModel.Instance.Setting.debugMode) MyMission?.StartMission(ScenarioSystem.Instance.targetMissionIndex);
        else MyMission?.StartMission(0);
    }

    public void OnEnd()
    {
        Logger.Log($"OnEnd {EventCode}");
        
        bool isObserver = ConfigModel.Instance.Setting.role == 0;
        Missions.ForEach(mission => mission.StopMission());
        if (production != null) production.OnAfterFinishMission(isObserver);
    }

    #endregion
}