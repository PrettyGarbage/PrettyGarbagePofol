using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Cysharp.Threading.Tasks;
using System.Linq;
using System.Threading;

public class AttachSystem : SceneContext<AttachSystem>
{
    Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags & (~Hand.AttachmentFlags.SnapOnAttach) & (~Hand.AttachmentFlags.DetachOthers) & (~Hand.AttachmentFlags.VelocityMovement);
    
    Dictionary<Interactable, (Vector3 originPos, Quaternion originRot)> cached = new();
    
    public void Attach(Interactable interactable, Hand hand)
    {
        if (interactable == null || hand == null)
        {
            Logger.LogError("Hand is null");
            return;
        }
        
        GrabTypes startingGrabType = hand.GetGrabStarting();

        var interactableTransform = interactable.transform;
        
        cached[interactable] = (interactableTransform.position, interactableTransform.rotation);

        interactableTransform.position = hand.transform.position;
        interactableTransform.rotation = hand.transform.rotation;

        // Attach this object to the hand
        hand.AttachObject(interactable.gameObject, startingGrabType, attachmentFlags);
    }
    
    public void Detach(Interactable interactable, Hand hand)
    {
        if (hand == null)
        {
            Logger.LogError("Hand or Interactable is null");
            return;
        }
        
        // Detach this object from the hand
        hand.DetachObject(interactable.gameObject);

        // Restore position/rotation
        var interactableTransform = interactable.transform;
        interactableTransform.position = cached[interactable].originPos;
        interactableTransform.rotation = cached[interactable].originRot;
        cached.Remove(interactable);
    }
    
}
