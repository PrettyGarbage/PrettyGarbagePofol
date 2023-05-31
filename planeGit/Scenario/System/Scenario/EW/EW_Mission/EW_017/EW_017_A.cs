using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class EW_017_A : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_EW_017_A_1;

    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[0]).AddTo();

            //선반 모델 지목 및 설정하기
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10).AddTo());
            NextMission();
        }).AddTo();

        //선반 닫히는 애니메이션
        OnBeginMission(1, true).Subscribe(async _ =>
        {
            await director_EW_017_A_1.PlayAsync();
       
            NextMission();
        }).AddTo();

        OnBeginMission(2).Subscribe(async _ =>
        {
            LastMissionComplete();
        }).AddTo(); 
        
    }

    #endregion
}