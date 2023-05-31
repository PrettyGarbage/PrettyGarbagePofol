using UnityEngine;
using System.Linq;
using Library.Manager;
using UniRx;
using UnityEngine.Playables;
using Constants = Common.Constants;

public class EL_012_A : Mission
{
    #region Fields
    
    [SerializeField] PlayableDirector director_EL_012_A_npc3;
    [SerializeField] PlayableDirector director_EL_012_A_npc3_SlideEnd;
    
    [SerializeField] PlayableDirector director_EL_012_A_npc4;
    [SerializeField] PlayableDirector director_EL_012_A_npc4_SlideEnd;
    
    [SerializeField] PlayableDirector director_EL_012_A_npc10;
    [SerializeField] PlayableDirector director_EL_012_A_npc10_SldieEnd;
    
    #endregion
    
    #region Override Methods
    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("처음 탈출하는 승객에게 지시하세요.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 10).AddTo();
            
            Logger.Log("밑에서 내려가는 사람들을 도와주세요! Stay At The Bottom! Help People Off!");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(1, true).Subscribe(async _ =>
        {
            Logger.Log("사람들 슬라이드로 탈출하는 애니");

            await director_EL_012_A_npc4.PlayAsync();
            await director_EL_012_A_npc4_SlideEnd.PlayAsync();
            
            await director_EL_012_A_npc3.PlayAsync();
            await director_EL_012_A_npc3_SlideEnd.PlayAsync();

            await director_EL_012_A_npc10.PlayAsync();
            await director_EL_012_A_npc10_SldieEnd.PlayAsync();

            NextMission();
        }).AddTo(); 
        
        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
        
    }
    #endregion
   
}