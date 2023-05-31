using UnityEngine;
using UniRx;
using UnityEngine.Playables;
using Constants = Common.Constants;

public class EL_010_B : Mission
{
    #region Fields

    [SerializeField] PlayableDirector director_EL_010_B_2;

    /*[SerializeField]  DoorModel overwingWindowL;
    [SerializeField] Transform npc6WingPoint;
    
    [SerializeField] Transform npc6BeforeWing;*/
    

    #endregion

    #region Override Methods

    public override void SetMission()
    {
        var npc6 = NPCListModel.Instance.Get(6);

        OnBeginMission(0).Subscribe(async _ =>
        {
            Logger.Log("탈출구를 개방하세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 5).AddTo());

            Logger.Log("R1 도어 열리지 않음(Jamming)");
            await SubtitleSystem.Instance.ShowSubtitleAsync(Dialogues[1], 10).AddTo();

            Logger.Log("R1 탈출구 불량");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[2], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(1).Subscribe(async _ =>
        {
            Logger.Log("창가에 앉은 승객들을 탈출 시키세요.");
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[3], 10).AddTo());

            Logger.Log("창가에 앉은 손님! 창밖을 보세요! 안전합니까? 손잡이를 당기세요!!Hey You! Look outside! Is it safe? Pull the handle!! ");
            MissionResults.Add(await ShoutingSystem.Instance.ShoutingMissionAsync(Dialogues[4], 10).AddTo());

            NextMission();
        }).AddTo();

        OnBeginMission(2, true).Subscribe(async _ =>
        {
            Logger.Log("6번 npc 지목 후 탈출구를 여는 애니 연출하기 열기");
            
            await director_EL_010_B_2.PlayAsync();

            NextMission();
        }).AddTo();

        OnBeginMission(3).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}