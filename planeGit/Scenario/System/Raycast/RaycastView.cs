using UnityEngine;

public class RaycastView : MonoBehaviour
{
    [SerializeField]
    private LineRenderer _lineRenderer;
    [SerializeField]
    private GameObject _marker;

    ///<summary>
    ///라인랜더러와 마커 표시
    ///</summary>
    public void ShowLines(Vector3 endPosition)
    {
        _marker.SetActive(true);
        _marker.transform.position = endPosition;
        _lineRenderer.enabled = true;
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, endPosition);
    }

    ///<summary>
    ///라인랜더러와 마커 숨기기
    ///</summary>
    public void HideLines()
    {
        _marker.SetActive(false);
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, transform.forward * 100);
    }
}
