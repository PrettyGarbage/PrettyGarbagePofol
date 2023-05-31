using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using Library.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class T7_002_Production : ScenarioEventProduction
{
    #region Fields

    [SerializeField] GameObject preview;
    GameObject XRCamera;
    #endregion
    
    #region Override Methods

    public override async UniTask OnPrevStartMission(bool isObserver)
    {        
        Logger.Log("T7_002 시작");

     
        XRCamera = GameObject.FindGameObjectWithTag("XRCamera");
        if (XRCamera != null)
        {
            XRCamera.gameObject.SetActive(false);
        }
        preview.SetActive(true);
        await UniTask.Delay(20000);
        preview.SetActive(false);
        
        if (XRCamera != null)
        {
            XRCamera.gameObject.SetActive(true);
        }
        
    }

    public override void OnAfterFinishMission(bool isObserver)
    {
        Logger.Log("T7_002 종료");
    }

    #endregion
}