using Common;
using UnityEngine;
using System;
using System.Linq;
using Library.Manager;
using UniRx;
using UnityEngine.Playables;

public class EL_011_D : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_EL_011_D_1;
    [SerializeField] PlayableDirector director_EL_011_D_3;
    [SerializeField] PlayableDirector director_EL_011_D_5;
    
    /*public InflationHandleModel R2infaltionHandle;
    [SerializeField] SlideModel R2Slide;
    [SerializeField] CabinetModel l2Cabinet;
    
    [SerializeField] Transform[] npc7waypoints;
    [SerializeField] Transform[] npc8waypoints;
    
    [SerializeField] Transform[] npc7waypointsSecond;*/

    #endregion
    
    #region Override Methods
    public override void SetMission()
    {
        var npc7 = NPCListModel.Instance.Get(7);
        var npc8 = NPCListModel.Instance.Get(8);
        
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("Manual inflation handle 당겨");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            NextMission();
        }).AddTo();
        
        OnBeginMission(1, true).Subscribe(async _ =>
        {
            Logger.Log("Manual inflation handle 당기는 애니 연출");
            await director_EL_011_D_1.PlayAsync();

            /*var audioSource1 = SoundManager.Instance.PlaySoundBGM("pin");
            audioSource1.Play();
            R2infaltionHandle.Animator.SetTrigger(Constants.InflationHandlePull);
            await R2infaltionHandle.Animator.WaitAnimationCompleteAsync(Constants.InflationHandle);
            
            var audioSource2 = SoundManager.Instance.PlaySoundBGM("OpenSlide");
            audioSource2.Play();
            
            Logger.Log("슬라이드 팽창되는 애니");
            R2Slide.gameObject.SetActive(true);
            R2Slide.Animator.SetTrigger(Constants.SlideOpen);
            await R2Slide.Animator.WaitAnimationCompleteAsync(Constants.Slide_Spread_it_out);*/
            NextMission();
        }).AddTo();
        
        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("슬라이드 팽창 확인");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());
            NextMission();
        }).AddTo();
        
        OnBeginMission(3, true).Subscribe(async _ =>
        {
            Logger.Log("일부 승객 가지고 R2 탈출구 방향으로 이동한다. ");

            NPCListModel.Instance.Gets(7, 8).ForEach(npc =>
            {
                npc.Animator.SetFloat(Constants.IdleState, 1);
            });
            
            await director_EL_011_D_3.PlayAsync();

            /*await NPCMovementSystem.Instance.MoveByPath(npc7, 1, npc7waypoints);
            npc7.transform.forward = npc7waypoints.Last().forward;
        
            npc7.Animator.SetTrigger(Constants.PutInOverheadbin);
            l2Cabinet.Animator.SetBool(Constants.OpenCabinetL, true);
            await npc7.Animator.WaitAnimationCompleteAsync(Constants.ManPutInOverheadbin);*/
            
            /*carrier.SetActive(true);
            await NPCMovementSystem.Instance.CarrierMoveByPath(npc7, 1, carrierwaypoints);*/

            NextMission();
        }).AddTo();
        
        OnBeginMission(4).Subscribe( async _ =>
        {
            Logger.Log("탈출구 정상!! 짐버려!! 이쪽으로!!Good Exit! Leave Everything! Come This Way!");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[2], 10).AddTo());

            NextMission();
        }).AddTo(); 
        
        OnBeginMission(5, true).Subscribe(async _ =>
        {
            Logger.Log("일부 승객이 L2 탈출구 방향으로 이동한다. ");
            await director_EL_011_D_5.PlayAsync();

            /*NPCMovementSystem.Instance.MoveByPath(npc8, 1, npc8waypoints);
            npc8.transform.forward = npc8waypoints.Last().forward;
            await NPCMovementSystem.Instance.MoveByPath(npc7, 1, npc7waypointsSecond);
            npc7.transform.forward = npc7waypointsSecond.Last().forward;            */
            NextMission();
            
        }).AddTo();

        OnBeginMission(6).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}