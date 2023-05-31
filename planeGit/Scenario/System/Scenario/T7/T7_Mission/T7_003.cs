using UnityEngine;
using System;
using Library.Manager;
using UniRx;
using UnityEngine.Playables;

public class T7_003 : Mission
{
    #region Fields

    //[SerializeField] GameObject checkList;

    #endregion
  
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("승무원은 지시대로 객실내부 구조를 확인하세요.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0],10).AddTo();
            Logger.Log("체크 항목을 확인하시고 준비 완료 시 START 버튼을 선택해 주세요.");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[1], 10).AddTo();

            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            //checkList.gameObject.SetActive(true);
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(_ => { LastMissionComplete(); }).AddTo();
    }

    #endregion
}