using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpritePinchZoomCtrl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    [SerializeField]
    private Image _image;

    [SerializeField]
    private float _maxZoomValue = 2f;
    [SerializeField]
    private float _zoomSpeed = 0.1f;

    [SerializeField]
    private float _curZoomValue = 1f;

    private Rect _adjustedRect;

    private RectTransform _rectTransform;

    private void Awake()
    {
        _adjustedRect = _image.GetPixelAdjustedRect();
        _rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        //_image.rectTransform.position = Vector3.zero;
        //_image.transform.localScale = Vector3.one;
        //_image.rectTransform.pivot = Vector2.one/2;
    }

    private void Update()
    {
        if (_image)
        {
#if UNITY_EDITOR
            if (Input.mousePresent)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 p = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                    SetPivot(p.x, p.y);
                }

                if (Input.GetMouseButton(0))
                    UpdateZoom(Input.mouseScrollDelta.y * _zoomSpeed);

                if (Input.GetMouseButtonUp(0))
                    SetPivot(0.5f, 0.5f);
            }
#endif
        }
    }

    private void UpdateZoom(float value)
    {
        if (value == 0) return;

        float nextScale = _rectTransform.localScale.x + value;
        if (nextScale > _maxZoomValue)
        {
            nextScale = _curZoomValue;
        }
        else if (_rectTransform.localScale.x < 1f)
        {
            nextScale = 1f;
        }

        _rectTransform.localScale = Vector3.one * nextScale;

        _curZoomValue = nextScale;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("OnBeginDrag : " + eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("OnEndDrag : " + eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag : " + eventData);
        //Vector3 newPoint = _image.transform.localPosition + new Vector3(eventData.delta.x, eventData.delta.y, 0f);
        //_image.transform.localPosition = newPoint;
    }

    private void SetPivot(float x, float y)
    {
        //if (point.x >0f && point.y > 0f)
        //{
        //float x = 0.5f -((0.5f - point.x) / _curZoomValue);
        //float y = 0.5f - ((0.5f - point.y) / _curZoomValue);
        //_image.rectTransform.pivot = new Vector2(x, y);
        //}

        _rectTransform.pivot = new Vector2(x, y);
    }
}

