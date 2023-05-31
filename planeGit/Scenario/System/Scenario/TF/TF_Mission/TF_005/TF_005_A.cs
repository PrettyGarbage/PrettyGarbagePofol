using Common;
using UnityEngine;
using System;
using UniRx;

public class TF_005_A : Mission
{
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {          
            Logger.Log("손님여러분, 지금 비행기 내에 작은 화재가 발생하여 저희 승무원들이 진압하고 있습니다. 저희 승무원들은 이런 경우에 대비하여 잘 훈련돼 있습니다. 손님여러분께서는 동요하지 마시고 침착하게 승무원의 지시에 따라 주시기 바랍니다.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0], 20).AddTo(); 
            
            Logger.Log("Ladies and Gentleman, a minor fire has broken out in the cabin, but it is now under control, Please do not be alarmed. We ask you to please follow the instructions of out cabin crew. Thank you for your coorperation.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[1], 20).AddTo();

            NextMission();
        }).AddTo();
        
        OnBeginMission(1).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}