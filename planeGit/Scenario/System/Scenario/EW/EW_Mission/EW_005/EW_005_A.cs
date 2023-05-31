using Common;
using Library.Manager;
using UniRx;
using UnityEngine;

public class EW_005_A : Mission
{
    #region Override Methods
    public override void SetMission()
    {
        
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("사운드 : 기장에게서 인터폰으로 연락이 옵니다. 인터폰을 받으세요");

             //hadset 모델에 등록하기 
             MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
             
            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            await HandsetSystem.Instance.HandsetMissionAsync(Dialogues[1], 30).AddTo();
            //await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[1]).AddTo();

            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            LastMissionComplete();
        }).AddTo();


    }

    #endregion
}
