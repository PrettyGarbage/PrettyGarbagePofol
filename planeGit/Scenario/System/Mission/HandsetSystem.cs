using System;
using System.Linq;
using System.Threading;
using Common;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class HandsetSystem : SceneContext<HandsetSystem>
{
    [SerializeField] Interactable handset;

    Vector3 oldPosition;
    Quaternion oldRotation;
    Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags & (~Hand.AttachmentFlags.SnapOnAttach) & (~Hand.AttachmentFlags.DetachOthers) & (~Hand.AttachmentFlags.VelocityMovement);
    string currentNumber;

    public async UniTask<CrewTrainingResultPacket.PacketData.Mission> HandsetMissionAsync(Dialogue dialogue, float waitSeconds = 10, bool autoDetach = true)
    {
        if (!handset)
        {
            Logger.LogError("Handset is null");
            return new CrewTrainingResultPacket.PacketData.Mission()
            {
                missionDescription = dialogue.Body.Select(body => body.Message).Aggregate((a, b) => $"{a} {b}"),
                result = (false).ToString(),
                resultDescription = "Handset is null"
            };
        }

        var cts = new CancellationTokenSource();

        Attach(handset, out var hand);
        
        var interactableObject = handset.GetComponent<InteractableObjectModel>();

        var winIndex = await UniTask.WhenAny(
            SubtitleSystem.Instance.ShowSubtitleAsync(dialogue, waitSeconds, cts.Token),             
            WaitInputAsync(interactableObject, cts)
        );

        cts.Cancel();

        interactableObject.IsInteractable = false;
        
        Detach(handset, autoDetach, hand);

        var missionResult = new CrewTrainingResultPacket.PacketData.Mission()
        {
            missionDescription = dialogue.Body.Select(body => body.Message).Aggregate((a, b) => $"{a} {b}"),
            result = (winIndex == 1).ToString(),
            resultDescription = (winIndex == 1) ? "성공" : "주어진 시간 안에 번호를 입력하지 못했습니다."
        };

        return missionResult;
    }
    public async UniTask AttachMissionAsync(Interactable item, Dialogue dialogue, float waitSeconds = 10, bool autoDetach = true)
    {
        try
        {
            var cts = new CancellationTokenSource();

            Attach(item, out var hand);

            await SubtitleSystem.Instance.ShowSubtitleAsync(dialogue, waitSeconds, cts.Token);

            Detach(item, autoDetach, hand);

            cts.Cancel();
        }
        catch
        {
            Logger.LogError($"{ScenarioSystem.Instance.CurrentScenario.currentScenarioEvent.EventCode}-{ScenarioSystem.Instance.CurrentScenario.currentEventIndex} : AttachMissionAsync Error");
        }
    }

    void Attach(Interactable item, out Hand hand)
    {
        hand = Valve.VR.InteractionSystem.Player.instance.rightHand;

        AttachSystem.Instance.Attach(item, hand);
    }

    void Detach(Interactable item, bool autoDetach, Hand hand)
    {        
        if (autoDetach) AttachSystem.Instance.Detach(item, hand);

        currentNumber = null;
    }

    public async UniTask WaitInputAsync(InteractableObjectModel interactableObject, CancellationTokenSource cts)
    {
        for (int i = 0; i < 3; i++)
        {
            interactableObject.IsInteractable = true;
            await UniTask.WaitUntil(() => interactableObject.IsInteractable == false, cancellationToken: this.GetCancellationTokenOnDestroy());
            if (cts.IsCancellationRequested) return;
        }
    }

    public void DetachHandset()
    {
        var hand = Valve.VR.InteractionSystem.Player.instance.rightHand;

        AttachSystem.Instance.Detach(handset, hand);
    }
}