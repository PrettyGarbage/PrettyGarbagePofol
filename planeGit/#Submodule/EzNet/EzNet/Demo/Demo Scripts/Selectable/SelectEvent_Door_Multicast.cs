using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzNetLibrary;

public class SelectEvent_Door_Multicast : MonoBehaviour
{
    public Animator animator;
    public bool isOpen = false;
    public Channel doorChannel;

    public void OnSelected()
    {
        isOpen = !isOpen;
        animator.SetBool("isOpen", isOpen);

        Debug.Log("OnSelected");
        EzNet.Add("isOpen", isOpen);
        EzNet.SendMulticast(doorChannel,
            GetComponent<EzNetObject>(), nameof(DoorStateRead));
    }
    public void OnDeselected()
    {
    }
    public void DoorStateRead()
    {
        isOpen = EzNet.Read<bool>("isOpen");
        animator.SetBool("isOpen", isOpen);
    }
}
