using Common;
using Library.Manager;
using UniRx;
using UnityEngine;

public class EW_030_ABCD : Mission
{
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0, true).Subscribe(async _ =>
        {
            Logger.Log("승객들이 구명정으로 올라타는 연출이 필요함");
      
            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("객실 및 화장실 내 잔류승객을 확인해주세요.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]).AddTo();
            
            Logger.Log("객실 및 화장실 내 잔류승객을 확인 하겠습니다.");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());

            NextMission();
        }).AddTo();   
        
        OnBeginMission(2).Subscribe(async _ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}