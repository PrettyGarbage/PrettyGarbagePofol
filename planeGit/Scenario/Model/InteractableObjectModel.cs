using System;
using HighlightPlus;
using MJ.Utils;
using UniRx;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(HighlightEffect))]
public class InteractableObjectModel : MonoBehaviour
{
    #region Fields

    bool isInteractable = true;
    HighlightEffect highlightEffect;
    
    #endregion

    #region Properties

    public bool IsInteractable
    {
        get => isInteractable;
        set
        {
            isInteractable = value;
            highlightEffect.highlighted = value;
            gameObject.layer = value ? LayerMask.NameToLayer("RaycastInteraction") : LayerMask.NameToLayer("DisableRaycastInteraction");
            GetComponent<Collider>().enabled = value;
        }
    }

    IObservable<InteractableObjectModel> OnTriggerLeftEvent => SteamVRInputSystem.Instance.OnTriggerStateDown
        .Where(action => action.GetStateDown(SteamVR_Input_Sources.LeftHand))
        .Where(_ => RaycastSystem.Instance.GetRayItem(0))
        .Select(_ => RaycastSystem.Instance.GetRayItem(0).GetComponent<InteractableObjectModel>())
        .Where(model => model)
        .Share();
    
    IObservable<InteractableObjectModel> OnTriggerRightEvent => SteamVRInputSystem.Instance.OnTriggerStateDown
        .Where(action => action.GetStateDown(SteamVR_Input_Sources.RightHand))
        .Where(_ => RaycastSystem.Instance.GetRayItem(1))
        .Select(_ => RaycastSystem.Instance.GetRayItem(1).GetComponent<InteractableObjectModel>())
        .Where(model => model)
        .Share();
    
    IObservable<InteractableObjectModel> OnTriggerEvent => OnTriggerLeftEvent.Merge(OnTriggerRightEvent).Share();

    #endregion

    #region Unity Lifecycle

    void Awake()
    {
        highlightEffect = gameObject.GetOrAddComponent<HighlightEffect>();
        gameObject.layer = LayerMask.NameToLayer("DisableRaycastInteraction");

        OnTriggerEvent.Subscribe(model =>
        {
            if (model != null) model.IsInteractable = false;
        }).AddTo();
    }

    #endregion
}
