using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CollectionItem : MonoBehaviour {

    [Header("Grade Color")]
    [SerializeField]
    private Color[] _gradeColor;
    [SerializeField]
    private Image _openFrameImage;

    [Header("BG")]
    [SerializeField]
    private GameObject _openObject;
    [SerializeField]
    private GameObject _closedObject;

    [Header("collection image")]
    [SerializeField]
    private Image _openImage;
    [SerializeField]
    private Image _closeImage;

    [Header("Unlock - Info")]
    [SerializeField]
    private GameObject _lockConditionObject;
    [SerializeField]
    private BuyButtonCtrl _buyButtonCtrl;
    [SerializeField]
    private LocalizationUIText _themeClearDescText;
    [SerializeField]
    private GameObject _lockImageObject;

    [Header("Open Enable ")]
    [SerializeField]
    private GameObject _openLockImageObject;
    [SerializeField]
    private Button _openButton;
    [SerializeField]
    private GameObject _openEnableAlramObject;

    [Header("Viewer ")]
    [SerializeField]
    private Button _viewButton;

    [Header("open animator")]
    [SerializeField]
    private Animator _openAnimator;

    private bool _isUserOpen = false;
    private bool _isOpenEnable = false;
    private CollectionInfo _collectionInfo;
    
    public bool IsOpenEnable { get { return _isOpenEnable; } }


    private void Initailize(CollectionInfo collectionInfo)
    {
        if (_collectionInfo != null) return;

        _collectionInfo = collectionInfo;
        
        _openFrameImage.color = _gradeColor[(int)collectionInfo.gradeType];

        DataManager.Instance.GetCollectionThumb(_collectionInfo.id, (sprite)=> {
            _openImage.sprite = sprite;
            _closeImage.sprite = sprite;
        });
        
        _openButton.onClick.AddListener(OnUserOpenCollection);
        _viewButton.onClick.AddListener(OnViewDetail);
        
    }

    public void SetCollectionItem(CollectionInfo collectionInfo)
    {

        Initailize(collectionInfo);
        ShowCollectionItem();
    }

    public void ShowCollectionItem()
    {

        if (_buyButtonCtrl.IsBuyStatus)
            return;

        _openObject.SetActive(false);
        _closedObject.SetActive(false);

       _openLockImageObject.SetActive(false);
        _lockConditionObject.SetActive(false);
        _buyButtonCtrl.gameObject.SetActive(false);
        _themeClearDescText.gameObject.SetActive(false);
        _openButton.gameObject.SetActive(false);
        _viewButton.gameObject.SetActive(false);

        _isOpenEnable = DataManager.Instance.IsOpenEnableCollection(_collectionInfo.id);
        _openEnableAlramObject.SetActive(_isOpenEnable);

        bool isOwn = DataManager.Instance.UserInfo.IsUserCollection(_collectionInfo.id);
        if (isOwn)
        {
            if (!_isUserOpen && _isOpenEnable)
            {
                _openLockImageObject.SetActive(true);
                _openButton.gameObject.SetActive(true);
                _closedObject.SetActive(true);
            }
            else
            {
                _viewButton.gameObject.SetActive(true);
                _openObject.SetActive(true);
            }
        }
        else
        {  
            _closedObject.SetActive(true);
            _lockConditionObject.SetActive(true);
            
            switch (_collectionInfo.unlockCondition.type)
            {
                case UnlockConditionType.SHOP:
                    ShowBuyButton(_collectionInfo.unlockCondition.typeId);
                    break;
                case UnlockConditionType.CLEAR_THEME_BASIC:
                    ShowUnlockThemeDesc(ThemeDifficulty.THEME_BASIC, _collectionInfo.unlockCondition.typeId);
                    break;
                case UnlockConditionType.CLEAR_THEME_MASTER:
                    ShowUnlockThemeDesc(ThemeDifficulty.THEME_MASTER, _collectionInfo.unlockCondition.typeId);
                    break;
                case UnlockConditionType.NONE:
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// 언락정보 - 테마클리어
    /// </summary>
    /// <param name="themeDifficulty"></param>
    /// <param name="themeId"></param>
    private void ShowUnlockThemeDesc(ThemeDifficulty themeDifficulty, string themeId)
    {
        _themeClearDescText.gameObject.SetActive(true);

        LocalizationTextKey localizationTextKey = themeDifficulty == ThemeDifficulty.THEME_BASIC ? LocalizationTextKey.COLLECTION_1_UNLOCKCONDITION_DESC : LocalizationTextKey.COLLECTION_2_UNLOCKCONDITION_DESC;

        _themeClearDescText.SetTextWithKeys(localizationTextKey, new String[] { GameManager.Instance.GetThemeNameKey(themeId) } );

    }

    /// <summary>
    /// 구매버튼 표시.
    /// </summary>
    /// <param name="productId"></param>
    private void ShowBuyButton(string productId)
    {
        _buyButtonCtrl.gameObject.SetActive(true);
        ProductInfo productInfo = DataManager.Instance.GetProductInfo(productId);
        _buyButtonCtrl.SetButton(productInfo, string.Empty, UnlockCollection);
    }

    /// <summary>
    /// 테마 클리어 보상 후 유저가 오픈버튼 클릭 후
    /// </summary>
    public void OnUserOpenCollection()
    {
        Debug.Log("OnUserOpenCollection " + _collectionInfo.id);
        _isUserOpen = true;
        _openButton.onClick.RemoveListener(OnUserOpenCollection);
        DataManager.Instance.RemoveUnlockCollectionId(_collectionInfo.id);
        UnlockCollection(true);
    }

    /// <summary>
    /// 콜렉션 오픈 연출.
    /// </summary>
    public void UnlockCollection(bool isBuyComplete)
    {
        if (isBuyComplete)
        {
            _lockImageObject.SetActive(false);
            _openLockImageObject.SetActive(true);
            SoundManager.Instance.PlayUISound(gameObject, AudioDataKey.ui_collection_unlock);
            _openButton.gameObject.SetActive(false);
            _openAnimator.Play("open", -1, 0);

        }
    }

    public void OnUnlockAnimationEvent()
    {
        ShowCollectionItem();
        SoundManager.Instance.PlayUISound(gameObject, AudioDataKey.ui_button_theme_card_flip);
    }

    public void HideAni()
    {
        _openAnimator.Play("hide", -1, 0);
    }
    public void ShowAni()
    {
        _openAnimator.Play("show", -1, 0);
    }

    /// <summary>
    /// 자세히 보기.
    /// </summary>
    private void OnViewDetail()
    {
        CollectionDetailState.Open(_collectionInfo, _gradeColor[(int)_collectionInfo.gradeType]);
    }
}
