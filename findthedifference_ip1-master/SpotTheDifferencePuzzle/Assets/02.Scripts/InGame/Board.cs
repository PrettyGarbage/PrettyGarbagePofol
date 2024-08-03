using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Board : MonoBehaviour
{
    const int INCREASE_PART_COUNT = 9;

    [SerializeField] DifferenceImageToken _imagePartPrefab;
    [SerializeField] Image _mainImage;
    [SerializeField] RectTransform _checkMarkRoot;
    [SerializeField] RectTransform _oppositeCheckMarkRoot;
    [SerializeField] RectTransform _oppositeBoard;

    [SerializeField] RectTransform _debugTokenMarker;

    [SerializeField] GameObject _comboObj;

    Queue<DifferenceImageToken> _tokenBuffer = new Queue<DifferenceImageToken>();
    Queue<RectTransform> _debugMarkerBuffer = new Queue<RectTransform>();

    List<DifferenceImageToken> _tokens = new List<DifferenceImageToken>();
    List<RectTransform> _debugMarkers = new List<RectTransform>();

    Action<int> _onTouchCorrect;
    Action<Vector2> _onTouchWrongPosition;
    Action<Vector2> _onStarTrailPos;

    Quaternion _originRotate;
    Quaternion _oppositeOriginRotate;

    //for multi touch
    [SerializeField] private RectTransform _oppositeSideRect;
    bool _isPinching = false; //현재 핀치줌 상태인지...
    Vector2 _touchPos;
    Vector2 _pivotVector;

    float _startPinchDistance; // 처음 두손가락의 거리
    float _startPinchZoom = 1; //첫 핀치 줌 배율
    public float _currentZoom = 1.0f;
    Vector2 _startPinchScreenPosition;
    Vector2 _startPinchCenterPosition; //중앙 포지션.

    private void Update()
    {
        //Debug.Log("터치 카운트 : " + Input.touchCount);

        if(Input.touchCount == 0 && _isPinching)
        {          
            _isPinching = false;
            InitZoom();
        }
    }

    public IEnumerator Initialize(Action<int> onTouchCorrect, Action<Vector2> onTouchWrongPosition, Action<Vector2> onStarTrailPos)
    {
        InGameVariables.PUZZLE_TOKEN_EXTRA_OFFSET.AddDelegate(value => {
            for (int i = 0, max = _debugMarkers.Count; i < max; ++i)
            {
                _debugMarkers[i].sizeDelta =
                    _tokens[i].image.rectTransform.sizeDelta + Vector2.one * value * 2.0f;
            }
        });

        InGameVariables.DISPLAY_DEBUG_TOKEN_MARKER.AddDelegate(value => {
            for (int i = 0, max = _debugMarkers.Count; i < max; ++i)
            {
                _debugMarkers[i].gameObject.SetActive(value);
            }
        });

        _onTouchCorrect = onTouchCorrect;
        _onTouchWrongPosition = onTouchWrongPosition;
        _onStarTrailPos = onStarTrailPos;

        _originRotate = transform.rotation;
        _oppositeOriginRotate = _oppositeBoard.transform.rotation;

        Input.multiTouchEnabled = true;

        yield return InitializeImage();
        yield return IncreaseParts();
    }

    public IEnumerator Setup(Sprite mainSprite, PuzzleImagePart[] parts, int index, int count)
    {
        _mainImage.sprite = mainSprite;

        ClearTokens();

        while (_tokenBuffer.Count < parts.Length)
        {
            yield return IncreaseParts();
        }

        for (int i = 0, max = parts.Length; i < max; ++i)
        {
            DifferenceImageToken token = _tokenBuffer.Dequeue();
            token.Setup(parts[i], (i >= index && i < index + count));

            RectTransform debugMarker = _debugMarkerBuffer.Dequeue();
            debugMarker.gameObject.SetActive(InGameVariables.DISPLAY_DEBUG_TOKEN_MARKER);
            debugMarker.localPosition = token.image.rectTransform.localPosition;
            debugMarker.localScale = token.image.rectTransform.localScale;
            debugMarker.sizeDelta = token.image.rectTransform.sizeDelta +
                Vector2.one * InGameVariables.PUZZLE_TOKEN_EXTRA_OFFSET * 2.0f * GameConstants.PUZZLE_IMAGE_SIZE_RATE;
            debugMarker.SetAsFirstSibling();

            _tokens.Add(token);
            _debugMarkers.Add(debugMarker);
        }
    }

    public void ClearTokens()
    {
        for (int i = 0, max = _tokens.Count; i < max; ++i)
        {
            _tokens[i].Release();
            _tokenBuffer.Enqueue(_tokens[i]);

            _debugMarkers[i].gameObject.SetActive(false);
            _debugMarkerBuffer.Enqueue(_debugMarkers[i]);
        }

        _tokens.Clear();
        _debugMarkers.Clear();
    }

    public bool Check(int tokenIndex)
    {
        if (tokenIndex < 0 ||
            tokenIndex >= _tokens.Count)
        {
            return false;
        }

        return _tokens[tokenIndex].Check();
    }

    public bool ShowHint(int tokenIndex)
    {
        if (tokenIndex < 0 ||
            tokenIndex >= _tokens.Count)
        {
            return false;
        }

        return _tokens[tokenIndex].ShowHint();
    }

    public void StopHint(int tokenIndex)
    {
        _tokens[tokenIndex].StopHint();
    }

    public DifferenceImageToken[] tokens
    {
        get
        {
            return _tokens.ToArray();
        }
    }

    #region COROUTINES
    IEnumerator InitializeImage()
    {
        EventTrigger imageEventTrigger = _mainImage.GetComponent<EventTrigger>();
        if (imageEventTrigger == null)
        {
            imageEventTrigger = _mainImage.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry imageTriggerEntry = new EventTrigger.Entry();
        imageTriggerEntry.eventID = EventTriggerType.PointerUp;
        imageTriggerEntry.callback.AddListener(eventData => {
            PointerEventData pointerEventData = eventData as PointerEventData;
            Vector2 position = _mainImage.rectTransform.InverseTransformPoint(pointerEventData.position);


            if (_isPinching || Input.touchCount > 1)
            {
                return;
            }

            int index = _tokens.FindIndex(token => token.IsIn(position));
            if (index >= 0)
            {
                if (_onTouchCorrect != null)
                {
                    _onTouchCorrect(index);
                    _onStarTrailPos(pointerEventData.position);
                }
            }
            else
            {
                if (_onTouchWrongPosition != null)
                {
                    if (pointerEventData.pointerCurrentRaycast.gameObject != null 
                    && !pointerEventData.pointerCurrentRaycast.gameObject.name.Equals(_mainImage.name))
                    {
                        return;
                    }

                    _onTouchWrongPosition(pointerEventData.position);
                }
            }
        });

        EventTrigger.Entry imageTriggerBtnDown = new EventTrigger.Entry();
        imageTriggerBtnDown.eventID = EventTriggerType.PointerDown;
        imageTriggerBtnDown.callback.AddListener(
            eventData =>
            {
                if (Input.touchCount == 2)
                {
                    if (!_isPinching)
                    {
                        _isPinching = true;
                        StartCoroutine(OnPinchStart());
                    }
                    if (_isPinching)
                    {
                        StartCoroutine(OnPinch());
                    }
                    _checkMarkRoot.gameObject.SetActive(false);
                    _oppositeCheckMarkRoot.gameObject.SetActive(false);
                    _comboObj.SetActive(false);                          
                }
            }
        );


        imageEventTrigger.triggers.Add(imageTriggerEntry);
        imageEventTrigger.triggers.Add(imageTriggerBtnDown);

        yield return new WaitForEndOfFrame();
    }

    IEnumerator IncreaseParts()
    {
        for (int i = 0; i < INCREASE_PART_COUNT; ++i)
        {
            DifferenceImageToken token = Instantiate(_imagePartPrefab);
            token.Initialize(
                _tokenBuffer.Count,
                _mainImage.rectTransform,
                _checkMarkRoot,
                _onTouchCorrect,
                _onTouchWrongPosition
            );

            _tokenBuffer.Enqueue(token);

            RectTransform debugMarker = Instantiate(_debugTokenMarker);
            debugMarker.SetParent(_mainImage.rectTransform);
            debugMarker.gameObject.name = "DebugMarker_" + _debugMarkerBuffer.Count;
            debugMarker.gameObject.SetActive(false);

            _debugMarkerBuffer.Enqueue(debugMarker);

            yield return new WaitForEndOfFrame();
        }
    }
    #endregion COROUTINES

    #region FOR PINCHZOOM
    IEnumerator OnPinchStart()
    {
        StateManager.Instance.ActiveBlockScreen(true);

        Vector2 pos1 = Input.touches[0].position;
        Vector2 pos2 = Input.touches[1].position;

        _startPinchDistance = Distance(pos1, pos2) * _mainImage.transform.localScale.x;
        _startPinchZoom = _currentZoom;
        _startPinchScreenPosition = (pos1 + pos2) / 2;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(_mainImage.rectTransform, _startPinchScreenPosition, null, out _startPinchCenterPosition);
        Vector2 pivotPosition = new Vector2(_mainImage.rectTransform.pivot.x * _mainImage.rectTransform.rect.size.x,
                                    _mainImage.rectTransform.pivot.y * _mainImage.rectTransform.rect.size.y);
        Vector2 posFromBottomLeft = pivotPosition + _startPinchCenterPosition;

        SetPivot(_mainImage.rectTransform,
            new Vector2(posFromBottomLeft.x / _mainImage.rectTransform.rect.width,
            posFromBottomLeft.y / _mainImage.rectTransform.rect.height));


        yield return null;
    }

    IEnumerator OnPinch()
    {
        bool isReleasePinchInSound = false;
        _startPinchZoom = 1f;

        while (_isPinching && Input.touchCount >= 1)
        {
            while (Input.touchCount == 1)
            {
                yield return null;
            }

            if (Input.touchCount == 0)
            {
                yield break;
            }

            float currentPinchDistance = Distance(Input.touches[0].position, Input.touches[1].position) * _mainImage.rectTransform.localScale.x;
            _currentZoom = (currentPinchDistance / _startPinchDistance) * _startPinchZoom;

            if(!isReleasePinchInSound && currentPinchDistance - _startPinchDistance > 100f)
            {
                isReleasePinchInSound = true;
                SoundManager.Instance.PlayUISoundInstance(AudioDataKey.ui_contents_zoom_in);
            }else if(isReleasePinchInSound && currentPinchDistance - _startPinchDistance < 100f
                && _currentZoom != 1f)
            {
                isReleasePinchInSound = false;
                SoundManager.Instance.PlayUISoundInstance(AudioDataKey.ui_contents_zoom_out);
            }

            _currentZoom = Mathf.Clamp(_currentZoom, 1f, 3.0f);

            //_checkMarkRoot.gameObject.SetActive(_currentZoom == 1f);
            //_oppositeCheckMarkRoot.gameObject.SetActive(_currentZoom == 1f);
            //_comboObj.SetActive(_currentZoom == 1f);

            _mainImage.transform.localScale = Vector3.one * _currentZoom;
            _oppositeSideRect.transform.localScale = Vector3.one * _currentZoom;
            yield return null;
        }
    }

    float Distance(Vector2 pos1, Vector2 pos2)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_mainImage.rectTransform, pos1, null, out pos1);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_mainImage.rectTransform, pos2, null, out pos2);

        return Vector2.Distance(pos1, pos2);
    }

    public void SetPivot(RectTransform rectTransform, Vector2 pivot)
    {
        if (rectTransform == null || pivot.x > 1 || pivot.y > 1
            || pivot.x < 0 || pivot.y < 0)
        {
            return;
        }

        Vector2 size = rectTransform.rect.size;
        rectTransform.pivot = pivot;

        _oppositeSideRect.pivot = pivot;
    }

    public void InitZoom()
    {
        float regressionSpeed = (_currentZoom == 1f) ? 0f : 0.2f; 

        LeanTween.value(_oppositeSideRect.gameObject.transform.localScale.x, 1f, 0.2f)
            .setOnStart(() =>
            {
                if (_mainImage.transform.localScale != Vector3.one)
                {
                    SoundManager.Instance.PlayUISoundInstance(AudioDataKey.ui_contents_zoom_out);
                }
            })
            .setOnUpdate((float val) =>
            {
                _mainImage.transform.localScale = new Vector3(val, val, val);
                _oppositeSideRect.transform.localScale = new Vector3(val, val, val);
            })
            .setOnComplete(()=>
            {
                _comboObj.SetActive(true);
                _checkMarkRoot.gameObject.SetActive(true);
                _oppositeCheckMarkRoot.gameObject.SetActive(true);
                _mainImage.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
                _oppositeSideRect.pivot = new Vector2(0, 0);
                StateManager.Instance.ActiveBlockScreen(false); //시작할때 블럭한거 풀어줘야함.
            })
            .setEaseOutExpo();
    }
    #endregion
}