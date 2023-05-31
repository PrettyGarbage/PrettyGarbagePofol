using Common;
using Library.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class EW_011_C : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_EW_011_C_2;

    #endregion
    
    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            await WaitOtherCrewMission(2, 1);

            NextMission();
        }).AddTo();


        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("지시 : Crewseat하단의 구명복을 착용한 후 승객들이 구명복을 착용하도록 안내해주세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(2, true).Subscribe(async _ =>
        {
            /*npc10.Animator.SetTrigger(Constants.EquipLifeJacket);
            npc10.Animator.OnAnimationCompleteAsObservable().Subscribe(_ => npc10.LifeJacket.gameObject.SetActive(true));*/

            await director_EW_011_C_2.PlayAsync();
            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(_ =>
        {
            NextMission();
        }).AddTo();   
        
        OnBeginMission(4).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}