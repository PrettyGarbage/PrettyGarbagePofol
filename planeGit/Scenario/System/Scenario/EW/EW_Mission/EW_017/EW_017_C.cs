using System.Collections;
using System.Collections.Generic;
using Common;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class EW_017_C : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_EW_017_C_2;

    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(1, 1);
            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]).AddTo();

            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10).AddTo());
            NextMission();
        }).AddTo();

        OnBeginMission(2, true).Subscribe(async _ =>
        {
            Logger.Log("열려있는 선반2 닫히는 연출");
            await director_EW_017_C_2.PlayAsync();
            
            /*l2Cabinet.Animator.SetBool(Constants.OpenCabinetL, false);*/
            
            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(async _ =>
        {
            NextMission();
        }).AddTo(); 
        
        OnBeginMission(4).Subscribe(async _ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}