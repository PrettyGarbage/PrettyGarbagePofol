using Common;
using Library.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class EW_011_B : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_EW_011_B_3;

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
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]).AddTo();
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[1], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            Logger.Log("사운드 : 유아용 구명복을 준비해주세요");
            
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[2], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(3, true).Subscribe(async _ =>
        {
            /*npc6.Animator.SetTrigger(Constants.WomanChildLifeJacket);
            npc11.Animator.SetTrigger(Constants.WomanChildLifeJacket);
            npc11.Animator.OnAnimationCompleteAsObservable().Subscribe(_ => npc11.LifeJacket.gameObject.SetActive(true));*/

            await director_EW_011_B_3.PlayAsync();
            NextMission();
        }).AddTo();

        OnBeginMission(4).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}