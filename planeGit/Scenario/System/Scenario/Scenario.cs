using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using Framework.Common.Template.SceneLoader;
using Library.Util.Reflection;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

public class Scenario : MonoBehaviour
{
    #region Properties

    [field: SerializeField] public List<ScenarioEvent> Missions { get; private set; } = new();
    public GenericStateMachine<Define.ScenarioState> stateMachine { get; } = new();
    public int currentEventIndex { get; private set; } = 0;
    public ScenarioEvent currentScenarioEvent => Missions[currentEventIndex];

    public List<CrewTrainingResultPacket.PacketData> CrewTrainingResultList { get; } = new();

    #endregion

    #region Public Methods

    ///<summary>
    ///시나리오의 초기 설정을 진행한다.
    ///Mission의 이름을 기반으로 Mission을 생성하여 등록하고 StateMachine을 생성한다.
    ///</summary>
    public void Initialize()
    {
        //Mission의 이름을 토대로 리플렉션으로 Mission 클래스를 생성하여 Missions에 추가
        Missions.ForEach(scenarioEvent => { scenarioEvent.Initialize(this); });

        //StateMachine을 생성
        stateMachine.OnBeginByState(Define.ScenarioState.Mission).Subscribe(_ => currentScenarioEvent.OnBegin());
        stateMachine.OnEndByState(Define.ScenarioState.Mission).Subscribe(_ =>
        {
            currentScenarioEvent.OnEnd();
            currentEventIndex++;
        });

        stateMachine.OnBeginByState(Define.ScenarioState.Complete).Subscribe(_ =>
        {
            Logger.Log("Scenario Complete");

            AllCrewTrainingResultPacket.SendPacket(new() { results = CrewTrainingResultList.ToArray() });

            DataModel.Instance.Mine.Status.Value = Define.Status.ScenarioEnd;
        });
    }

    ///<summary>
    ///미션을 처음부터 시작한다.
    ///</summary>
    public void StartMission()
    {
        if (stateMachine.IsActive) return;
        //디버깅용 코드
        if (ConfigModel.Instance.Setting.debugMode) currentEventIndex = ScenarioSystem.Instance.targetEventIndex;
        else currentEventIndex = 0;
        stateMachine.Start(Define.ScenarioState.Mission);
    }

    ///<summary>
    ///다음 미션이 존재할 경우 다음 미션을 시작하고 존재하지 않을 경우 완료 처리한다.
    ///</summary>
    public void Next()
    {
        if (currentEventIndex + 1 < Missions.Count) stateMachine.Transition(Define.ScenarioState.Mission);
        else stateMachine.Transition(Define.ScenarioState.Complete);
    }

    #endregion
}