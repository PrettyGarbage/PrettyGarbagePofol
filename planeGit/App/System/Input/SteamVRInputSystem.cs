using System;
using UniRx;
using UnityEngine;
using Valve.VR;

public class SteamVRInputSystem : SceneContext<SteamVRInputSystem>
{
    #region Fields

    [field: SerializeField] public SteamVR_Action_Boolean Trigger { get; private set; }

    #endregion
    
    #region Properties

    public IObservable<SteamVR_Action_Boolean> OnTriggerStateDown => Observable.EveryUpdate().Select(_ => Trigger).Where(_ => SteamVR.active).Where(trigger => trigger.activeBinding).Where(action => action.GetStateDown(SteamVR_Input_Sources.Any)).TakeUntilDestroy(gameObject).Share();
    public IObservable<SteamVR_Action_Boolean> OnTriggerState => Observable.EveryUpdate().Select(_ => Trigger).Where(_ => SteamVR.active).Where(trigger => trigger.activeBinding).Where(action => action.GetState(SteamVR_Input_Sources.Any)).TakeUntilDestroy(gameObject).Share();
    public IObservable<SteamVR_Action_Boolean> OnTriggerStateUp => Observable.EveryUpdate().Select(_ => Trigger).Where(_ => SteamVR.active).Where(trigger => trigger.activeBinding).Where(action => action.GetStateUp(SteamVR_Input_Sources.Any)).TakeUntilDestroy(gameObject).Share();

    #endregion

    protected override void Awake()
    {
        base.Awake();
        
        //OnTriggerStateDown.Subscribe(_ => Logger.Log("Trigger Down"));
        //OnTriggerStateDown.Where(action => action.GetStateDown(SteamVR_Input_Sources.RightHand)).Subscribe(_ => Logger.Log("Right Trigger Down"));
    }
}