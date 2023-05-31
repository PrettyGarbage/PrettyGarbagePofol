using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class MoveTweenBehaviour : PlayableBehaviour
{
    [HideInInspector] public MoveTweenModel model;
    public bool tweenPosition = true;
    public bool tweenRotation = true;
    public bool LookAtLastDirection = true;
    public bool isReverse = false;
    
    [HideInInspector] public Vector3 startingPosition;
    [HideInInspector] public TimelineClip clip;

    public override void PrepareFrame (Playable playable, FrameData info)
    {
        if (model != null && model.Path.Length > 0)
        {
            startingPosition = model.Path.First().position;
        }
    }
}
