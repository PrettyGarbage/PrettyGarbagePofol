using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAniBehaviour : StateMachineBehaviour {

    public Action<AnimatorStateInfo> onStateEnter;
    public Action<AnimatorStateInfo> onStateExit;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if (onStateEnter != null)
        {
            onStateEnter(stateInfo);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        if (onStateExit != null)
        {
            onStateExit(stateInfo);
        }
    }


}
