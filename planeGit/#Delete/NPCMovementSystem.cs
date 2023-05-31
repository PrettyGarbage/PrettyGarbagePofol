using System.Linq;
using Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class NPCMovementSystem : SceneContext<NPCMovementSystem>
{
    #region Public Methods

    public async UniTask MoveByPath(NPCModel npc, float speed, params Transform[] wayPoint)
    {
        npc.Animator.SetBool(Constants.IsSit, false);
        await npc.Animator.WaitAnimationCompleteAsync();
        npc.Animator.SetBool(Constants.IsWalk, true);
        await npc.transform.DOPath(wayPoint.Select(tr => tr.position).ToArray(), speed, PathType.Linear, PathMode.Full3D, gizmoColor: Color.red).SetLookAt(0.1f).SetSpeedBased().ToUniTask();
        npc.Animator.SetBool(Constants.IsWalk, false);
    }
    public async UniTask RaftMoveByPath(NPCModel npc1, NPCModel npc2, float speed, Transform[] npc1npc2wayPoint)
    {        
        npc1.Animator.SetBool(Constants.IsSit, false);
        npc2.Animator.SetBool(Constants.IsSit, false);
        await npc1.Animator.WaitAnimationCompleteAsync();
        await npc2.Animator.WaitAnimationCompleteAsync();
        npc1.Animator.SetBool(Constants.IsWalkOnRaftFront, true);
        npc2.Animator.SetBool(Constants.IsWalkOnRaftBack, true); 
        
        npc1.transform.DOPath(npc1npc2wayPoint.Select(tr => tr.position).ToArray(), speed, PathType.Linear, PathMode.Full3D, gizmoColor: Color.red).SetLookAt(0.1f).SetSpeedBased().ToUniTask();
        await npc2.transform.DOPath(npc1npc2wayPoint.Select(tr => tr.position).ToArray(), speed, PathType.Linear, PathMode.Full3D, gizmoColor: Color.red).SetLookAt(0.1f).SetSpeedBased().ToUniTask();
        npc1.Animator.SetBool(Constants.IsWalkOnRaftFront, false);
        npc2.Animator.SetBool(Constants.IsWalkOnRaftBack, false);
    }
    
    public async UniTask CarrierMoveByPath(NPCModel npc, float speed, params Transform[] wayPoint)
    {
        npc.Animator.SetBool(Constants.IsSit, false);
        await npc.Animator.WaitAnimationCompleteAsync();
        npc.Animator.SetBool(Constants.IsCarrierWalk, true);
        await npc.transform.DOPath(wayPoint.Select(tr => tr.position).ToArray(), speed, PathType.Linear, PathMode.Full3D, gizmoColor: Color.red).SetLookAt(0.1f).SetSpeedBased().ToUniTask();
        npc.Animator.SetBool(Constants.IsCarrierWalk, false);
    }  
    
    public async UniTask OceanJumpMoveByPath(NPCModel npc, float speed, params Transform[] wayPoint)
    {
        npc.Animator.SetBool(Constants.IsSit, false);
        await npc.Animator.WaitAnimationCompleteAsync();
        npc.Animator.SetTrigger(Constants.JumpOcean);
        await npc.transform.DOPath(wayPoint.Select(tr => tr.position).ToArray(), speed, PathType.Linear, PathMode.Full3D, gizmoColor: Color.red).ToUniTask();
        await npc.Animator.WaitAnimationCompleteAsync(Constants.Man_JumpOcean);
    }
    public async UniTask SwimMoveByPath(NPCModel npc, float speed, params Transform[] wayPoint)
    {
        npc.Animator.SetTrigger(Constants.ManSwim);
        await npc.transform.DOPath(wayPoint.Select(tr => tr.position).ToArray(), speed, PathType.Linear, PathMode.Full3D, gizmoColor: Color.red).SetLookAt(0.1f).SetSpeedBased().ToUniTask();
    }
    
#endregion
}

