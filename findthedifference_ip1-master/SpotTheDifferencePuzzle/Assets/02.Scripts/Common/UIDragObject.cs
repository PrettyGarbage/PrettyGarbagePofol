using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UIDragObject : MonoBehaviour , IBeginDragHandler, IEndDragHandler, IDragHandler
{

    public Action<PointerEventData> onDrageStart;
    public Action<PointerEventData> onDrageEnd;

    public void OnEndDrag(PointerEventData eventData)
    {
        if (onDrageEnd != null)
        {
            onDrageEnd(eventData);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (onDrageStart != null)
        {
            onDrageStart(eventData);
        }
        
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
    }
}
