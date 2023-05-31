using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class AD_003_B : Mission
{
    #region Fields
    //[SerializeField] PlayableDirector director_003_B;
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
            Logger.Log("L1 탑승구 위치에 서주세요. ");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[0], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(2, true).Subscribe(async _ =>
        {
            Logger.Log("L1 탑승구 위치에 발자국 표시 나타난다. (텔포 써야할것 같은데?)");
            //await director_003_B.PlayAsync();
            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
        
    }
    #endregion
}