using Common;
using UnityEngine;
using System;
using UniRx;

public class DE_007_A : Mission
{
    #region Fields
    /*
    [SerializeField] ParticleSystem smog;
    */
    [SerializeField] GameObject handsetIcon;

    #endregion
    
    #region Override Methods
    public override void SetMission()
    {
        OnBeginMission(0, true).Subscribe(async _ =>
        {
            Logger.Log("비행기 뒷편 구멍, 안개, 파편이 날아다니는 것을 표현안개(흰연기)로 비행기의 위험상황 표현");
            /*
            smog.Play();
            */
            
            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("Handset으로 기장에게 상황을 설명하세요.");
            handsetIcon.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            handsetIcon.gameObject.SetActive(false);

            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log(" 기장님, 사무장입니다. R2 Door 쪽에 구멍이 발생하였으며 파편이 날아다니고 안개가 심하게 발생하였습니다. 객실내 산소마스크를 착용하고 있습니다. ");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 20).AddTo());

            NextMission();
        }).AddTo();
        
        OnBeginMission(3).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}