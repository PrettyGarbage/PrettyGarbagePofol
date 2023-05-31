using Common;
using Library.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;
using Valve.VR.InteractionSystem;

public class EW_024_D : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_EW_024_D_1;
    [SerializeField] Interactable flashLight;

    #endregion
    
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {  
            Logger.Log("승무원 좌석 하단에 Flashlight를 들고 지정된 탈출구로 이동하여 승객들을 탈출시켜 주세요.");
      
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());

            NextMission();
        }).AddTo();   
        
        OnBeginMission(1, true).Subscribe(async _ =>
        {
            Logger.Log("jumpseat 하단 열리는 연출");
            await director_EW_024_D_1.PlayAsync();
            
            NextMission();
        }).AddTo();
        
        OnBeginMission(2).Subscribe(async _ =>
        {  
            Logger.Log("FlashLight를 선택하세요.");
      
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10).AddTo());

            NextMission();
        }).AddTo();   
        
        OnBeginMission(3, true).Subscribe(async _ =>
        {
            AttachSystem.Instance.Attach(flashLight, Valve.VR.InteractionSystem.Player.instance.rightHand);
            NextMission();
        }).AddTo();
        
        OnBeginMission(4).Subscribe(async _ =>
        {
            LastMissionComplete();
        }).AddTo();

    }

    #endregion
}
