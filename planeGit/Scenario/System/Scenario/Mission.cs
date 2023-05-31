using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using CoolishUI;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public abstract class Mission : MonoBehaviour
{
    #region Fields

    bool mine;
    [SerializeField] int testNo;

    #endregion

    #region Properties

    [field: SerializeField] public Define.CrewType TargetCrew { get; private set; }

    [field: SerializeField] public Dialogue[] Dialogues { get; private set; }
    public int Role { get; private set; }
    public string EventCode { get; private set; }
    public int MissionCode => Machine.CurrentState;
    GenericStateMachine<int> Machine { get; } = new();
    
    protected CrewTrainingResultPacket.PacketData TrainingResult { get; } = new();
    
    protected List<CrewTrainingResultPacket.PacketData.Mission> MissionResults { get; } = new();
    

    #endregion

    #region Public Methods

    public void Initialize(string eventCode, int role)
    {
        Role = role;
        mine = ConfigModel.Instance.Setting.role == role;
        EventCode = eventCode;
        
        TrainingResult.eventCode = EventCode;
        TrainingResult.testNo = testNo;
        TrainingResult.role = Role.ToString();
        TrainingResult.playerID = "";
        
        Machine.OnBegin.Subscribe(missionCode => DataModel.Instance.USModels.Mine().MissionCode.Value = missionCode);
        Machine.OnBegin.Subscribe(missionCode => Logger.Log($"CC{Role}-{EventCode}-{missionCode} 미션 시작"));
        Machine.OnEnd.Subscribe(missionCode => Logger.Log($"CC{Role}-{EventCode}-{missionCode} 미션 종료"));
    }

    public void StartMission(int initState)
    {
        TrainingResult.eventStartTime = DateTime.UtcNow.ToString("o");
        Machine.Start(initState);
    }

    public void StopMission() => Machine.Stop();

    #endregion

    #region Protected Methods

    protected async UniTask WaitOtherCrewMission(int missionCode, params int[] roles)
    {
        if (!mine) return;
        if (ConfigModel.Instance.Setting.debugMode) return;

        foreach (var role in roles)
        {
            if (!DataModel.Instance.USModels[role].IsConnected.Value) continue;
            await DataModel.Instance.USModels[role].MissionCode
                .Where(code => code >= missionCode)
                .FirstOrDefault()
                .ToUniTask(true, cancellationToken: ScenarioSystem.Instance.gameObject.GetCancellationTokenOnDestroy());
        }
    }

    protected void SendTrainingResult()
    {
        if (!mine) return;
        if (!MissionResults.Any()) return;

        Logger.Log($"SendTrainingResult {EventCode} {testNo}");

        TrainingResult.eventEndTime = DateTime.UtcNow.ToString("o");
        TrainingResult.missions = MissionResults.ToArray();
        TrainingResult.testNo = ++ScenarioSystem.Instance.testNo;
        TrainingResult.result = (MissionResults.All(x => x.result.ToLower() == "true")).ToString();
        
        ScenarioSystem.Instance.CurrentScenario.CrewTrainingResultList.Add(TrainingResult);
        
        CrewTrainingResultPacket.SendPacket(TrainingResult);
    }

    protected void LastMissionComplete()
    {
        if (!mine) return;

        SendTrainingResult();
        DataModel.Instance.USModels.Mine().EventStatus.Value = Define.EventStatus.End;
        Logger.Log("미션 종료 신호 전송");
        if (ConfigModel.Instance.Setting.debugMode) ScenarioSystem.Instance.CurrentScenario.Next();
    }

    protected IObservable<int> OnBeginMission(int targetMission, bool isSync = false)
    {
        if (ConfigModel.Instance.Setting.debugMode || !isSync || mine) return Machine.OnBeginByState(targetMission).Share();
        return OnSyncFunction(targetMission);
    }

    protected IObservable<int> OnEndMission(int targetMission, bool isSync = false)
    {
        if (ConfigModel.Instance.Setting.debugMode || !isSync || mine) return Machine.OnEndByState(targetMission).Share();
        return OnSyncFunction(targetMission);
    }

    protected void NextMission()
    {
        if (!mine) return;

        Machine.Transition(Machine.CurrentState + 1);
    }

    #endregion

    #region Abstract Methods

    public abstract void SetMission();

    #endregion

    #region Private Methods

    IObservable<int> OnSyncFunction(int targetState) => DataModel.Instance.USModels[Role].MissionCode.FirstOrDefault(code => code == targetState).Share();

    #endregion
}