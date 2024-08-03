using UnityEngine;

using TMPro;

public class NumberedHexaCell : HexaCell
{
    [SerializeField] TMP_Text _numberText;
    [SerializeField] SpriteMask _specularMask;

    int _number = 0;
    int _rotateStep = 0;
    float _angle = 0.0f;

    #region UNITY EVENTS
    protected override void Awake()
    {
        _specularMask.sprite = defaultSprite;
    }
    #endregion UNITY EVENTS

    public override void Initialize()
    {
        GameManager.instance.onThemeChanged += OnSwapTheme;
    }

    public override void Release()
    {
        GameManager.instance.onThemeChanged -= OnSwapTheme;
    }

    public override bool IsEqual(HexaCell cell)
    {
        NumberedHexaCell numberedCell = cell as NumberedHexaCell;
        if (numberedCell == null)
        {
            return false;
        }

        return (number == numberedCell.number);
    }

    public void Setup(int number, int rotateStep = 0)
    {
        _number = number;
        _rotateStep = rotateStep;
        _angle = _rotateStep * GameConstants.HEXA_STEP_ANGLE;

        OnSwapTheme(GameManager.instance.theme);

        //
        _numberText.LeanCancel();
        _numberText.text = GameManager.instance.displayNumberToPower ? Mathf.Pow(2, _number).ToString() : _number.ToString();
        _numberText.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, _angle);

        _specularMask.gameObject.SetActive(_number == GameConstants.HIGHEST_CELL_NUMBER);
    }

    public override void Placed()
    {
        _numberText.LeanCancel();
        _numberText.transform.rotation = Quaternion.identity;
    }

    public void Increase()
    {
        Setup(number + 1);
        Placed();
    }

    public void RotateCW(float duration)
    {
        --_rotateStep;

        Rotate(duration);
    }

    public void RotateCCW(float duration)
    {
        ++_rotateStep;

        Rotate(duration);
    }

    public int number
    {
        get
        {
            return _number;
        }
    }

    void Rotate(float duration)
    {
        float to = _rotateStep * GameConstants.HEXA_STEP_ANGLE;

        _numberText.LeanCancel();
        _numberText.LeanValue(_angle, to, duration).
            setEaseInOutBack().
            setOnUpdate(angle => {
                _angle = angle;
                _numberText.transform.localRotation =
                    Quaternion.Euler(0.0f, 0.0f, angle);
            });
    }

    void OnSwapTheme(BoardTheme theme)
    {
        if (theme == null)
        {
            return;
        }

        CellSprite sprite = theme.GetCellSprite(_number);
        if (sprite != null)
        {
            spriteRenderer.sprite = sprite.sprite;

            _specularMask.sprite = sprite.sprite;
            _numberText.gameObject.SetActive(sprite.displayNumber);
        }
        else
        {
            spriteRenderer.sprite = defaultSprite;

            _specularMask.sprite = defaultSprite;
            _numberText.gameObject.SetActive(true);
        }
    }
}