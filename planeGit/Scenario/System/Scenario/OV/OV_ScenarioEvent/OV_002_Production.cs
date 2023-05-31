using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class OV_002_Production : ScenarioEventProduction
{
    #region Fields
    [SerializeField] PlayableDirector director_OverwingWindow_1;
    [SerializeField] PlayableDirector director_OverwingWindow_2;
    [SerializeField] PlayableDirector director_OverwingWindow_3;
    [SerializeField] PlayableDirector director_OverwingWindow_4;
    #endregion
    
    #region Override Methods
    public override async UniTask OnPrevStartMission(bool isObserver)
    {
        Logger.Log("OV_002 시작");
        
        Logger.Log("Overwing Window을 개방하세요.");
        await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10);  
        
        Logger.Log("위의 손잡이 커버를 열어주세요.");
        await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10);    
        await director_OverwingWindow_1.PlayAsync();

        Logger.Log("아래의 손잡이 커버를 열어주세요.");
        await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[2], 10);  
        await director_OverwingWindow_2.PlayAsync();

        Logger.Log("위의 손잡이를 당겨주세요.");
        await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[3], 10);
        await director_OverwingWindow_3.PlayAsync();

        Logger.Log("창문을 밀어서 개방해주세요.");
        await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[4], 10);
        await director_OverwingWindow_4.PlayAsync();

    }
    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("OV_002 종료");
    }

    #endregion
}