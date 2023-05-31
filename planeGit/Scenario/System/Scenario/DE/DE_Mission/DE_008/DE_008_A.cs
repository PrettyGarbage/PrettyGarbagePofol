using Common;
using UnityEngine;
using System;
using Library.Manager;
using UniRx;

public class DE_008_A : Mission
{
    #region Fields

    [SerializeField] GameObject handsetIcon;

    #endregion
    
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            SoundManager.Instance.PlaySoundEffect("reseiveHandset");

            Logger.Log("Handset을 받으세요");
            handsetIcon.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            handsetIcon.gameObject.SetActive(false);

            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("기장입니다.안전고도에 도달했습니다. 승무원들은 객실상태를 점검바랍니다.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[1]).AddTo();

            NextMission();
        }).AddTo();
        
        OnBeginMission(2).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}