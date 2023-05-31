using UnityEngine;
using System.Linq;
using UniRx;
using UnityEngine.Playables;
using Constants = Common.Constants;


//B의 미션 적음 및 동선 상 추가했음
public class EL_013_B : Mission
{
    #region Fields
    [SerializeField] PlayableDirector director_EL_013_B_1_npc1;
    [SerializeField] PlayableDirector director_EL_013_B_1_npc1_SlideEnd;
    
    [SerializeField] PlayableDirector director_EL_013_B_1_npc2;
    [SerializeField] PlayableDirector director_EL_013_B_1_npc2_SlideEnd;
    
    #endregion
    
    #region Override Methods

    public override void SetMission()
    {
      OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("13B_0번 미션 진입");
            await WaitOtherCrewMission(2, 1); 
            NextMission();
        }).AddTo();
        
        OnBeginMission(1, true).Subscribe(async _ =>
        {
            NPCListModel.Instance.Gets(1, 2).ForEach(npc =>
            {
                npc.Animator.SetFloat(Constants.IdleState, 1);
            });
            
            await director_EL_013_B_1_npc2.PlayAsync();
            await director_EL_013_B_1_npc2_SlideEnd.PlayAsync();
            
            await director_EL_013_B_1_npc1.PlayAsync();
            await director_EL_013_B_1_npc1_SlideEnd.PlayAsync();
            
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
        
    }
    #endregion
   
}