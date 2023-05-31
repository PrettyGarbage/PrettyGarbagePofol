using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class AD_005_C : Mission
{
    [SerializeField] PlayableDirector director_005_C_1;
    [SerializeField] PlayableDirector director_005_C_2;

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
            Logger.Log("비상구 좌석 승객에게 브리핑하세요.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 10).AddTo();

            Logger.Log("손님 이 좌석은 비상 좌석으로 비상시 저희 승무원을 도와 탈출을 도와주실 수 있으시겠습니까?");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(2, true).Subscribe(async _ =>
        {
            await director_005_C_1.PlayAsync();
            NextMission();
        }).AddTo();  
        
        OnBeginMission(3).Subscribe(async _ =>
        {
            Logger.Log("자세한 사항은 앞 좌석 안 안내서를 참고하시기 바랍니다.");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[2], 10).AddTo());
            NextMission();
        }).AddTo();
        
        OnBeginMission(4, true).Subscribe(async _ =>
        {
            await director_005_C_2.PlayAsync();
            NextMission();
        }).AddTo();  

        OnBeginMission(5).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}