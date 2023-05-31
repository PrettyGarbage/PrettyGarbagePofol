using System;
using System.Linq;
using Common;
using UnityEngine;

public abstract class PlayerBase : MonoBehaviour
{
    protected virtual void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    #region method

    public abstract VideoCapture GetVideoCapture();

    #endregion
}
