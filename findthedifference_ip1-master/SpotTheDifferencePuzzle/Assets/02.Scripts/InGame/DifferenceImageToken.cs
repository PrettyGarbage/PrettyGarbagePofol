using System;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DifferenceImageToken : MonoBehaviour
{
    [SerializeField] Image _image;
    [SerializeField] Image _hintMark;
    [SerializeField] Image _insideHintMark;
    [SerializeField] GameObject _checkMarkObject;
    [SerializeField] float _hidingTime = 0.5f;
    [SerializeField] float _blinkingTime = 0.5f;

    public bool isChecked { get; private set; }
    public bool isHinted { get; private set; }

    int _index;
    float _distance;
    Action<int> _onTouchCorrect;
    Action<Vector2> _onTouchWrongPosition;

    #region UNITY EVENTS
    void OnDisable()
    {
        _hintMark.color *= GameConstants.TRANSPARENT;
        _insideHintMark.color *= GameConstants.TRANSPARENT;
        _checkMarkObject.SetActive(false);
    }
    #endregion UNITY EVENTS

    public void Initialize(
        int index,
        RectTransform tokenRoot,
        RectTransform markerRoot,
        Action<int> onTouchCorrect,
        Action<Vector2> onTouchWrongPosition)
    {
        //
        _index = index;

        //
        gameObject.name = "Part_" + _index;

        //
        _image.rectTransform.SetParent(tokenRoot);
        _image.rectTransform.localScale = Vector3.one;
        _image.rectTransform.localEulerAngles = Vector3.zero;

        //
        _hintMark.gameObject.name = "HintMarker_" + _index;
        _hintMark.rectTransform.SetParent(markerRoot);
        _hintMark.rectTransform.localScale = Vector3.one;
        _hintMark.color = GameConstants.TRANSPARENT;

        _checkMarkObject.gameObject.name = "CheckMarker_" + _index;
        _checkMarkObject.transform.SetParent(markerRoot);
        _checkMarkObject.SetActive(false);


        gameObject.SetActive(false);
    }

    public void Setup(PuzzleImagePart part, bool isVisible)
    {
        isChecked = false;
        isHinted = false;
        
        gameObject.SetActive(true);

        _image.LeanCancel();
        
        _image.rectTransform.localPosition = part.Position * GameConstants.PUZZLE_IMAGE_SIZE_RATE;
        _image.rectTransform.localScale = Vector2.one * GameConstants.PUZZLE_IMAGE_SIZE_RATE;
        _image.sprite = part.PartSprite;
        _image.color = isVisible ? Color.white : GameConstants.TRANSPARENT;
        _image.SetNativeSize();

        _hintMark.rectTransform.position = _image.rectTransform.position;
        _checkMarkObject.transform.position = _image.rectTransform.position;
    }

    public void Release()
    {
        gameObject.SetActive(false);

        _hintMark.LeanCancel();        
    }

    public bool IsIn(Vector2 position)
    {
        if (isChecked)
        {
            return false;
        }

        float radius = (_image.rectTransform.sizeDelta.x * 0.5f + InGameVariables.PUZZLE_TOKEN_EXTRA_OFFSET) * GameConstants.PUZZLE_IMAGE_SIZE_RATE;
        float distance = Vector2.Distance(_image.rectTransform.localPosition, position);

        return (distance <= radius);
    }

    public bool Check()
    {
        if (isChecked)
        {
            return false;
        }

        isChecked = true;

        _image.LeanAlpha(1.0f, _hidingTime).
            setIgnoreTimeScale(true);


        if (isHinted)
        {
            isHinted = false;
            _hintMark.color = GameConstants.TRANSPARENT;
            _insideHintMark.color = GameConstants.TRANSPARENT;
            _hintMark.LeanCancel();
            _insideHintMark.LeanCancel();
        }

        _checkMarkObject.SetActive(true);

        return true;
    }

    public bool ShowHint()
    {
        if (isChecked || isHinted)
        {
            return false;
        }

        isHinted = true;

        _hintMark.rectTransform.LeanAlpha(0.8f, _blinkingTime).
            setIgnoreTimeScale(true).
            setEaseInOutBack().
            setLoopPingPong();

        _insideHintMark.rectTransform.LeanAlpha(0.8f, _blinkingTime).
            setIgnoreTimeScale(true).
            setEaseInOutBack().
            setLoopPingPong();

        return true;
    }

    public void StopHint()
    {
        _hintMark.LeanCancel();
        _insideHintMark.LeanCancel();
    }

    public Image image
    {
        get
        {
            return _image;
        }
    }
}