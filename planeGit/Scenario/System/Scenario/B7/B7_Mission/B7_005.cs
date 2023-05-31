using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine.Playables;

public class B7_005 : Mission
{
    #region Fields

    [SerializeField] GameObject BeltIcon;
    [SerializeField] GameObject lockedBeltIcon;
    
    [SerializeField] PlayableDirector director_005_Locked;
    [SerializeField] PlayableDirector director_005_Normal;
    #endregion

    #region Override Methods

    public override void SetMission()
    {
        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("안전벨트를 착용해보세요.");
            BeltIcon.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            BeltIcon.gameObject.SetActive(false);

            NextMission();
        }).AddTo(); 
        
         OnBeginMission(1).Subscribe(async _ =>
         {
             await director_005_Locked.PlayAsync();
            NextMission();
        }).AddTo(); 
         
         OnBeginMission(2).Subscribe(async _ =>
         {
             Logger.Log("안전벨트를 풀어보세요.");
             lockedBeltIcon.gameObject.SetActive(true);
             MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10).AddTo());
             lockedBeltIcon.gameObject.SetActive(false);

            NextMission();
        }).AddTo(); 
         
         OnBeginMission(3).Subscribe(async _ =>
         {
             await director_005_Normal.PlayAsync();
             NextMission();
        }).AddTo(); 
        
        OnBeginMission(4).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}