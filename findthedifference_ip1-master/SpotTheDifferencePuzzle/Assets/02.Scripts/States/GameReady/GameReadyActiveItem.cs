using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameReadyActiveItem : MonoBehaviour {

    [SerializeField]
    private ItemType _itemType;

    [SerializeField]
    private Image _itemImage;

    [SerializeField]
    private Button _shopButton;

    [SerializeField]
    private TMP_Text _textItemAmount;

    private void Start()
    {
        _shopButton.onClick.AddListener(OnGoShop);
    }

    private void OnEnable()
    {
        UpdateAmount();
        _itemImage.sprite = ResourceDataManager.Instance.GetItemResourceData(_itemType).Sprite;
        EventPool.Listen(GetInstanceID(), EventNames.ON_UPDATE_USERINFO, UpdateCurrency);
    }

    private void OnDisable()
    {
        EventPool.Remove(GetInstanceID(), EventNames.ON_UPDATE_USERINFO, UpdateCurrency);
    }

    private void UpdateCurrency(object[] args)
    {
        UpdateAmount();
    }

    public void UpdateAmount()
    {
        _textItemAmount.text = DataManager.Instance.UserInfo.GetItemCount(_itemType).ToString();
    }

    public void OnGoShop()
    {
        ItemShopState.Open(_itemType);
        SoundManager.Instance.PlayUISound(gameObject, AudioDataKey.ui_button_common);
    }

}
