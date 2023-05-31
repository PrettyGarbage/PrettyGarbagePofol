using Common;
using Library.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class EW_005_C : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_EW_005_C_2;

    #endregion
    
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(2, 1); //1번이 2번 미션할때까지 대기

            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("사운드 : 안전벨트를 착용하지 못한 승객들의 안전벨트를 착용 시키세요 ");

            //7번 npc pointout 등록하기
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(2, true).Subscribe(async _ =>
        {
            /*npc7.Animator.SetTrigger(Constants.EquipBelt);
            npc7.Seat.Belt.Animator.SetBool(Constants.IsEquipBelt, true);
            await npc7.Animator.WaitAnimationCompleteAsync();*/
            
            await director_EW_005_C_2.PlayAsync();

            NextMission();
        }).AddTo(); 
        
        OnBeginMission(3).Subscribe(async _ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}