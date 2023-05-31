using UnityEngine;
using System;
using UniRx;
using UnityEngine.Playables;

public class TF_005_B : Mission
{
    #region Fields

    [SerializeField] GameObject halonPBEIcon;
    [SerializeField] GameObject halonIcon;
    [SerializeField] PlayableDirector director_005_B;
    
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
            Logger.Log("PBE를 착용하시고 hanlon 소화기로 화재를진압하세요.");
            halonPBEIcon.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[0], 10).AddTo());
            halonPBEIcon.gameObject.SetActive(false);
            NextMission();
        }).AddTo();

        OnBeginMission(2, true).Subscribe(async _ =>
        {
            Logger.Log("PBE를 착용 및 소화기 드는 연출");

            NextMission();
        }).AddTo();
        
        OnBeginMission(3, true).Subscribe(async _ =>
        {
            Logger.Log("화장실 문을 여는 연출하기");
            Logger.Log("소화기 노즐이 들어갈 정도로 살짝만 화장실 문이 열린다.");
            Logger.Log("화장실 내에서 하얀 연기가 뿜어져 나온다.");
            
            
            Logger.Log("위 아래로 분사하기 연출");
            Logger.Log("화장실 쓰레기통 화재  후 하얀 연기가 사라진다.");

            await director_005_B.PlayAsync();
            NextMission();
        }).AddTo();   
        
        OnBeginMission(4).Subscribe(async _ =>
        {
            Logger.Log("재발화를 막기위해 충분히 소화액을 분사하세요.");
            halonIcon.gameObject.SetActive(true);
            MissionResults.Add(await PointOutSystem.Instance.PointOutMissionAsync(Dialogues[1], 10).AddTo());
            halonIcon.gameObject.SetActive(false);
            NextMission();
        }).AddTo();  
        
        OnBeginMission(5).Subscribe(async _ =>
        {
            Logger.Log("소화액을 든다.");
            Logger.Log("화재 진압 완료.");
            
            NextMission();
        }).AddTo();
        

        OnBeginMission(6).Subscribe(_ =>
        {
            LastMissionComplete();
        }).AddTo();
    }

    #endregion
}