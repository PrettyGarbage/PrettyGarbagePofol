using UnityEngine;
using System.Collections;


public class MyGizmos : MonoBehaviour {

    [SerializeField]
    public Color _color = Color.red;

    [SerializeField]
    [Range(0,1)]
    public float _size = 0.2f;

    public bool isRay = false;

    public void OnDrawGizmos()
    {
        Gizmos.color = _color;
        Gizmos.DrawSphere(transform.position, _size);

        if(isRay)Gizmos.DrawRay(transform.position, transform.forward);
    }
}
