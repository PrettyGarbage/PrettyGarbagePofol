using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;
using Valve.VR.InteractionSystem;

public class EL_009_A : Mission
{
    #region Fields
    
    [SerializeField] PlayableDirector director_EL_009_A_1;
    [SerializeField] Interactable flashLight;

    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("승무원 좌석 하단의 Flashlight들 들고 지정된 L1 탈출구로 이동하여 승객들을 탈출 시켜주세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(1, true).Subscribe(async _ =>
        {
            await director_EL_009_A_1.PlayAsync();
            
            // 손전등이 자동으로 On 상태 되도록 연출하기

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
            // 손전등이 자동으로 On 상태 되도록 연출하기
            AttachSystem.Instance.Attach(flashLight, Valve.VR.InteractionSystem.Player.instance.rightHand);
            NextMission();
        }).AddTo(); 

        OnBeginMission(4).Subscribe(async _ =>
        {
            UniTask.Delay(2000);
            Logger.Log("L1탈출구 이동 위치 표시 노출");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[2], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(5).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}