using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCModel : MonoBehaviour
{
    #region Properties

    public int id;
    public Animator Animator { get; private set; }

    #endregion

    #region Unity Lifecycle

    void Awake()
    {
        Animator = GetComponentInChildren<Animator>();
    }

    #endregion
}
