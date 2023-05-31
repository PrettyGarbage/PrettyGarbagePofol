using UniRx;
using UnityEngine;
using Valve.VR;

public class HandPresenter : MonoBehaviour
{
    [SerializeField] SteamVR_Input_Sources input;
    [SerializeField] Renderer renderer;
    
    void Awake()
    {
        var animator = GetComponent<Animator>();
        
        SteamVRInputSystem.Instance.OnTriggerStateDown.Where(action => action.GetStateDown(input)).Subscribe(_ =>
        {
            animator.SetInteger("HandAniState", 1);
        }).AddTo(gameObject);
        
        SteamVRInputSystem.Instance.OnTriggerStateUp.Where(action => action.GetStateUp(input)).Subscribe(_ =>
        {
            animator.SetInteger("HandAniState", 0);
        }).AddTo(gameObject);

        Observable.EveryUpdate()
            .Where(_ => Valve.VR.InteractionSystem.Player.instance.GetHand(input == SteamVR_Input_Sources.LeftHand ? 0 : 1).AttachedObjects.Count > 0)
            .Subscribe(_ =>renderer.enabled = false)
            .AddTo(gameObject);
        
        Observable.EveryUpdate()
            .Where(_ => Valve.VR.InteractionSystem.Player.instance.GetHand(input == SteamVR_Input_Sources.LeftHand ? 0 : 1).AttachedObjects.Count == 0)
            .Subscribe(_ => renderer.enabled = true)
            .AddTo(gameObject);
    }
}
