using UnityEngine;
using System;
using Library.Manager;
using UniRx;
using UnityEngine.Playables;
using Valve.VR.InteractionSystem;

public class EE_003 : Mission
{
    #region Fields

    //[SerializeField] PlayableDirector director_003_1;
    [SerializeField] Interactable flashLight;

    #endregion
  
    #region Override Methods
    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("Flashlight를 확인하세요.");
            await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0],10).AddTo();

            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            //flash light 깜빡임
            //await director_003_1.PlayAsync();
            Logger.Log("Flashlight :정위치, 충전지시등 점멸 상태를 확인하세요.");

            await HandsetSystem.Instance.AttachMissionAsync(flashLight ,Dialogues[1],10).AddTo();
            
            NextMission();
        }).AddTo();
        
        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}