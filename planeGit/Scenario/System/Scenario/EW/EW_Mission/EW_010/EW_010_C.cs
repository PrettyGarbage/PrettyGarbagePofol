using Common;
using Library.Manager;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class EW_010_C : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_EW_010_C_2;
    
    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(2, 1);

            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            // 노트북 모델 등록하기
            //result1 = await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10, false).AddTo();
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[0], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(2, true).Subscribe(async _ =>
        {
            //1번 승객이 노트북 모델 overheadbin으로 올리기
            Logger.Log("연출 : 1번 승객이 노트북을 overheadbin으로 올리기");
            await director_EW_010_C_2.PlayAsync();

            NextMission();
        }).AddTo();
        
        OnBeginMission(3).Subscribe(async _ =>
        {
            NextMission();
        }).AddTo();  
        
        OnBeginMission(4).Subscribe(async _ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}
