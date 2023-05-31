using Common;
using Library.Manager;
using UniRx;

public class EW_006_A : Mission
{
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]).AddTo();
            
            //todo: 수화기를 드는 트리거 구현 필요
            
            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
           await WaitOtherCrewMission(2, 2, 3, 4).AddTo(); //2, 3, 4번 승무원이 2번 미션을 시작할 때까지 대기
            
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());

            NextMission();
        }).AddTo();  

        OnBeginMission(3).Subscribe(async _ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}

