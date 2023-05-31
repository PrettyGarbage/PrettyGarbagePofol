using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    public void Select()
    {
        gameObject.SendMessage("OnSelected");
    }
    public void Deselect()
    {
        gameObject.SendMessage("OnDeselected");
    }
}
