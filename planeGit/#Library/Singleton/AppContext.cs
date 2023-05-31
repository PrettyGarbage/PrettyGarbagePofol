using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AppContext<T> : SceneContext<T> where T : AppContext<T>
{
    #region Unity Lifecycle

    protected override void Awake()
    {
        base.Awake();
        transform.parent = null;
        DontDestroyOnLoad(gameObject);
    }

    #endregion
}
