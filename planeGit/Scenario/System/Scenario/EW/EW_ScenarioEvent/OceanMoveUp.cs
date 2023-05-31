using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanMoveUp : MonoBehaviour
{
    [SerializeField] float moveTime = 30f;
    [SerializeField] float startHeight = -1f;
    [SerializeField] float endHeight = 0f;

    void Awake()
    {
        transform.SetPosY(startHeight);
    }

    public void OceanMove()
    {
        transform.DOLocalMove(new Vector3(0, endHeight, 0f), moveTime);
    }
}