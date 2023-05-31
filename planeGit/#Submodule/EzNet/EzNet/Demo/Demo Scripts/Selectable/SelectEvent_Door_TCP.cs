using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzNetLibrary;

public class SelectEvent_Door_TCP : MonoBehaviour
{
    public Animator animator;
    public bool isOpen = false;
    public void OnSelected()
    {
        isOpen = !isOpen;
        animator.SetBool("isOpen", isOpen);

        Debug.Log("OnSelected");

        if (EzNet.TCP_SERVER.isServer())
        {
            EzNet.Add("isOpen", isOpen);
            EzNet.SendTCP(EzNet.SendTo.Client, EzNet.WhereSpace.MySpace,
                GetComponent<EzNetObject>(), nameof(DoorStateRead));
        }
        else
        {
            EzNet.Add("isOpen", isOpen);
            EzNet.SendTCP(EzNet.SendTo.Host, EzNet.WhereSpace.MySpace,
                GetComponent<EzNetObject>(), nameof(DoorStateRead_Host));
        }
    }
    public void OnDeselected()
    {
    }
    public void DoorStateRead_Host()
    {
        isOpen = EzNet.Read<bool>("isOpen");
        animator.SetBool("isOpen", isOpen);

        EzNet.Add("isOpen", isOpen);
        EzNet.SendTCP(EzNet.SendTo.Client, EzNet.WhereSpace.MySpace,
            GetComponent<EzNetObject>(), nameof(DoorStateRead));
    }
    public void DoorStateRead()
    {
        isOpen = EzNet.Read<bool>("isOpen");
        animator.SetBool("isOpen", isOpen);
    }
}
