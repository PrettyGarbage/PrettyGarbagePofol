using System;
using System.Collections;
using UniRx;
using UnityEngine;

public class RaycastPresenter : MonoBehaviour
{
    public float _length = 10f;
    private RaycastHit hit;
    public IObservable<RaycastHit> OnColliderEnter => Observable.EveryUpdate()
        .Select(_ => hit)
        .Where(action => action.collider != null)
        .Share();
    public IObservable<RaycastHit> OnColliderExit => Observable.EveryUpdate()
        .Select(_ => hit)
        .Where(action => action.collider == null)
        .Share();

    ///<summary>
    ///래이캐스트 체크 시작
    ///</summary>
    public void StartRaycastCheck()
    {
        if(gameObject.activeInHierarchy) StartCoroutine(IRayCast());
    }

    public void StopRaycast()
    {
        StopAllCoroutines();
    }

    private IEnumerator IRayCast()
    {
        while (true)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            Debug.DrawRay(transform.position, 100* transform.forward);
            Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("RaycastInteraction"));
            yield return null;
        }
    }
}
