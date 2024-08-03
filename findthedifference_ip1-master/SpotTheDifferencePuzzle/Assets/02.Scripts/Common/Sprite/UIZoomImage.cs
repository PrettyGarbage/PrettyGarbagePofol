using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIZoomImage : MonoBehaviour
{
    public enum TouchStatus
    {
        NONE, ONE, TWO
    }

    public TouchStatus touchStatus = TouchStatus.NONE;

    [SerializeField]
    private float _zoomSpeed = 1.0f;

    private float _maxZoom = 5f;

    [SerializeField]
    private float _backScaleTime = 0.1f;

    [SerializeField]
    private RectTransform _rectTransform;

    [SerializeField]
    private RectTransform _viewPortRectTransform;

    private Canvas _canvas;

    private bool _isZoomEnable;

    private Vector2 _touchCentorPoint;
    private Touch _touchZero, _touchOne;
    private Vector2 _touchZeroPrevPos = Vector2.zero, _touchOnePrevPos = Vector2.zero;
    private float _prevTouchDeltaMag, _touchDeltaMag, _deltaMagnitudeDiff;
    private Vector3 _updateScaleDelta, _updateScaleNextScale;
    //move
    private Touch _touch;
    private Vector2 _imageMoveForDiffPos = Vector2.zero;
    private float _xbound, _ybound;
    private Vector3 _nextPos;
    private Vector3 _lastPos;

    private void OnEnable()
    {
        _isZoomEnable = true;
        _maxZoom = GameConstants.COLLECTION_MAX_ZOOM;

        _rectTransform.localScale = Vector3.one;
        _rectTransform.localPosition = Vector3.zero;

        if(!_canvas)
            _canvas = transform.root.GetComponent<Canvas>();

    }

    void Update()
    {


        if (!_isZoomEnable)
        {
            return;
        }

        if (SystemUtil.IsDoubleTap())
        {
            ResetPosition(Vector3.zero);
            ResetScale(Vector3.one);
            return;
        }

        if (Input.touchCount == 2 &&  _rectTransform.localScale.x >= 1f && _rectTransform.localScale.x <= _maxZoom)
        {
            touchStatus = TouchStatus.TWO;
            _touchZero = Input.GetTouch(0);
            _touchOne = Input.GetTouch(1);

            if (_touchCentorPoint == Vector2.zero && (_deltaMagnitudeDiff > 0.1f || _deltaMagnitudeDiff < -0.1f))
            {
                _touchCentorPoint = (_touchOne.position + _touchZero.position) / 2;

                Vector2 localpoint;
                
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_viewPortRectTransform, _touchCentorPoint, _canvas.worldCamera, out localpoint);

                Vector2 normalizedPoint = Rect.PointToNormalized(_viewPortRectTransform.rect, localpoint);

                _viewPortRectTransform.pivot = normalizedPoint;

            }

            // Find the position in the previous frame of each touch.
            _touchZeroPrevPos = _touchZero.position - _touchZero.deltaPosition;
            _touchOnePrevPos = _touchOne.position - _touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            _prevTouchDeltaMag = (_touchZeroPrevPos - _touchOnePrevPos).magnitude;
            _touchDeltaMag = (_touchZero.position - _touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            _deltaMagnitudeDiff = _prevTouchDeltaMag - _touchDeltaMag;

            if((_deltaMagnitudeDiff > 0.1f || _deltaMagnitudeDiff < -0.1f))
            {
                UpdateScale(-_deltaMagnitudeDiff * _zoomSpeed);
            }
            
        }

        if (Input.touchCount == 1 && touchStatus != TouchStatus.TWO)
        {
            touchStatus = TouchStatus.ONE;
            _touch = Input.GetTouch(0);
            if (_touch.phase == TouchPhase.Began && _imageMoveForDiffPos == Vector2.zero)
            {
                _imageMoveForDiffPos = (_touch.position - new Vector2(_rectTransform.localPosition.x, _rectTransform.localPosition.y));
                _lastPos = _rectTransform.localPosition;
            }

            _nextPos = _touch.position - _imageMoveForDiffPos;
            _rectTransform.localPosition = _nextPos;

            //Debug.Log("nextPos : " + nextPos + ", xbound: " + xbound + ", ybound: " + ybound);
            if (IsVaildBound(_nextPos))
            {
                _lastPos = _nextPos;
            }
            else
            {
                if (_touch.phase == TouchPhase.Ended)
                {
                    ResetPosition(_lastPos);                    
                }
            }
        }

        if (Input.touchCount == 0)
        {  

            if (touchStatus == TouchStatus.TWO)
            {
                Vector3 pos = _rectTransform.position;
                _rectTransform.localScale *= _viewPortRectTransform.localScale.x;

                if(_rectTransform.localScale.x > _maxZoom)
                {
                    _rectTransform.localScale = Vector3.one * _maxZoom;
                }
                else if (_rectTransform.localScale.x < 1f)
                {
                    _rectTransform.localScale = Vector3.one;
                }

                _viewPortRectTransform.localScale = Vector3.one;
                _viewPortRectTransform.pivot = Vector2.one / 2;
                _viewPortRectTransform.localPosition = Vector2.zero;
                _rectTransform.position = pos;

                if (_rectTransform.localScale.x <= 1.1f || !IsVaildBound(_rectTransform.localPosition))
                {
                    ResetPosition(Vector3.zero);
                }
            }
            _imageMoveForDiffPos = Vector2.zero;
            _touchCentorPoint = Vector2.zero;
            _deltaMagnitudeDiff = 0f;
            touchStatus = TouchStatus.NONE;
        }
    }

    private void ResetPosition(Vector3 point)
    {
        _isZoomEnable = false;
        LeanTween.moveLocal(_rectTransform.gameObject, point, _backScaleTime).setOnComplete(() =>
        {
            _isZoomEnable = true;
        }); ;
    }

    private void ResetScale(Vector3 scale)
    {
        _isZoomEnable = false;
        LeanTween.scale(_rectTransform.gameObject, scale, _backScaleTime).setOnComplete(() =>
        {
            _isZoomEnable = true;
        }); ;
    }

    private void UpdateScale(float scaleValue)
    {
        _updateScaleDelta = Vector3.one * scaleValue;
        _updateScaleNextScale = _viewPortRectTransform.localScale + _updateScaleDelta;

        if (_updateScaleNextScale.x > _maxZoom / _rectTransform.localScale.x)
        {
            _updateScaleNextScale = Vector3.one * _maxZoom / _rectTransform.localScale.x;
        }

        if(_updateScaleNextScale.x < 1f / _rectTransform.localScale.x)
        {
            _updateScaleNextScale = Vector3.one / _rectTransform.localScale.x;           
        }

        _viewPortRectTransform.localScale = _updateScaleNextScale;        
    }

    private bool IsVaildBound(Vector3 point)
    {
        _xbound = (_rectTransform.rect.width * _rectTransform.localScale.x / 2) - (Screen.currentResolution.width / 2);
        _ybound = (_rectTransform.rect.height * _rectTransform.localScale.y / 2) - (Screen.currentResolution.height / 2);

        _xbound *= 1.05f;
        _ybound *= 1.05f;

        return Math.Abs(point.x) <= _xbound && Math.Abs(point.y) <= _ybound;
    }

}
