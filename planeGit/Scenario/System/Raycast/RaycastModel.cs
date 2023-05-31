using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastModel
{
    private Vector3 _endPosition = Vector3.zero;
    public Vector3 EndPosition { get=>_endPosition;}

    private bool _isRay = false;
    public bool IsRay { get=>_isRay; }

    private GameObject _rayObj;
    public GameObject RayObj { get => _rayObj; }
    ///<summary>
    ///콜라이더 들어옴 => 래이캐스트 당한 결과물 캐싱
    ///</summary>
    public void SetRayRslt(Vector3 endposition, GameObject hitObj)
    {
        _isRay = true;
        _endPosition = endposition;
        _rayObj = hitObj;
    }

    ///<summary>
    ///콜라이더 나감 => 래이캐스트 결과물 삭제
    ///</summary>
    public void RemoveRayRslt()
    {
        _endPosition = Vector3.zero;
        _isRay = false;
        _rayObj = null;
    }
}
