using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ReadyItemCtrl : MonoBehaviour {

    [SerializeField]
    private ItemType _itemType;

    [SerializeField]
    private Image _itemImage;

    [SerializeField]
    private GameObject _useObject;
    [SerializeField]
    private GameObject _buyObject;

    [SerializeField]
    private TMP_Text _count;
    [SerializeField]
    private Button _btnBuy;
    [SerializeField]
    private Button _btnUse;

    [SerializeField]
    private bool _isUseCheck;
    [SerializeField]
    private GameObject _useCheckObject;

    private UserItemInfo _userItemInfo;

    public bool IsUseCheck { get { return _isUseCheck; } }

    public ItemType ItemType { get { return _itemType; } }


    private void OnEnable()
    {
        _btnBuy.onClick.AddListener(OnBuy);
        _btnUse.onClick.AddListener(OnUse);
        EventPool.Listen(GetInstanceID(), EventNames.ON_UPDATE_USERINFO, SetItemInfo);
    }

    private void OnDisable()
    {
        _btnBuy.onClick.RemoveListener(OnBuy);
        _btnUse.onClick.RemoveListener(OnUse);
        EventPool.Remove(GetInstanceID(), EventNames.ON_UPDATE_USERINFO, SetItemInfo);
    }

    public void SetItem(ItemType itemType)
    {
        _itemType = itemType;
        SetItemInfo();
        SetCheck(false);

        if (PlayerPrefs.GetString(_itemType.ToString()).Equals(GameConstants.GAME_READY_PASSIVE_ITEM_USE)
                && _userItemInfo.count > 0)
        {
            OnUse();
        }

    }

    private void OnUse()
    {
        if(_userItemInfo!=null && _userItemInfo.count > 0)
        {
            SetCheck(!_isUseCheck);
            SoundManager.Instance.PlayUISound(gameObject, _isUseCheck ? AudioDataKey.ui_toggle_on : AudioDataKey.ui_toggle_off);
        }
    }

    private void OnBuy()
    {
        ItemShopState.Open(_itemType);
    }

    private void SetItemInfo(object[] args = null)
    {
        _userItemInfo = DataManager.Instance.UserInfo.GetItem(_itemType);

        _itemImage.sprite = ResourceDataManager.Instance.GetItemResourceData(_itemType).Sprite;

        if (_userItemInfo != null)
        {
            _count.text = SystemUtil.GetCommaText(_userItemInfo.count);
            SetVisibleObject(_userItemInfo.count>0);
        }
        else
        {
            _count.text = CommonConstants.ZERO_STRING;
            SetVisibleObject(false);
        }
    }

    private void SetVisibleObject(bool isUsable)
    {
        _useObject.SetActive(isUsable);
        _buyObject.SetActive(!isUsable);
    }

    private void SetCheck(bool isCheck)
    {
        _isUseCheck = isCheck;
        _useCheckObject.SetActive(isCheck);
    }

}
