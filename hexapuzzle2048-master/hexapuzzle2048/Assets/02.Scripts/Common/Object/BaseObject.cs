using UnityEngine;
using System.Collections;
using System;

public abstract class BaseObject : MonoBehaviour
{
    public abstract void Dispose();

    private void OnDestroy()
    {
        EventPool.RemoveAll(GetInstanceID());
        Dispose();
    }

}