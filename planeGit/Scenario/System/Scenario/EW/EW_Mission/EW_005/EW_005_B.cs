using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class EW_005_B : Mission
{
    #region Fields
    
    [SerializeField] PlayableDirector director_EW_005_B_2;
    [SerializeField] PlayableDirector director_EW_005_B_4;
    
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
            //2번 npc에 pointout 등록하기
            
            Logger.Log("화장실에서 나오는 손님을 제자리로 이동시키세요");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            
            Logger.Log("자리에 앉으시고 안전벨트를 착용하세요. ");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(2, true).Subscribe(async _ =>
        {
            /*await NPCMovementSystem.Instance.MoveByPath(npc2, 1, waypoints);
            npc2.transform.forward = waypoints.Last().forward;

            npc2.Animator.SetBool(Constants.IsSit, true);
            await npc2.Animator.WaitAnimationCompleteAsync();
            npc2.Seat.Belt.Animator.SetBool(Constants.IsEquipBelt, true);
            npc2.Animator.SetTrigger(Constants.EquipBelt);*/
            
            await director_EW_005_B_2.PlayAsync();

            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(async _ =>
        {
            Logger.Log("사운드 : 안전벨트를 착용하지 못한 어린이의 안전벨트를 착용시키세요. ");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[2], 10).AddTo());
            
            Logger.Log("안전벨트를 착용하세요.");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[3], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(4, true).Subscribe(async _ =>
        {
            /*npc6.Animator.SetTrigger(Constants.WomanChildSeatBelt);
            npc11.Animator.SetTrigger(Constants.WomanChildSeatBelt);
            npc11.Seat.Belt.Animator.SetBool(Constants.IsEquipBelt, true);
            await npc11.Seat.Belt.Animator.WaitAnimationCompleteAsync(Constants.BeltOn);*/
            
            await director_EW_005_B_4.PlayAsync();
            
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(5).Subscribe(async _ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}