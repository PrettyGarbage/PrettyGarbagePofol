using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzNetLibrary;

public class SelectEvent_Door : MonoBehaviour
{
    public Animator animator;
    public bool isOpen = false;
    public void OnSelected()
    {
        isOpen = !isOpen;
        animator.SetBool("isOpen", isOpen);
        EzNet.Add("isOpen", isOpen);
        EzNet.SendBroadcast(EzNet.WhereSpace.MySpace, GetComponent<EzNetObject>(), nameof(DoorStateRead));
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
