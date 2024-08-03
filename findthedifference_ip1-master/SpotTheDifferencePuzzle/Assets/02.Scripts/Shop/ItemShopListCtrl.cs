using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class ItemShopListCtrl : ShopListCtrl
{
    [Header("Item")]
    [SerializeField]
    private ItemType _itemType;

    [SerializeField]
    private LocalizationUIText _descText;

    private ItemShopStateData _itemShopStateData;

    public override void SetProductList(ShopState shopState)
    {
        base.SetProductList(shopState);
        _itemShopStateData = new ItemShopStateData(_shopType, _itemType);

        switch (_itemType)
        {
            case ItemType.SPIN_TICKET:
            case ItemType.UNLIMIT_HEART_30MIN:                
            case ItemType.BONUS_TIME:
                break;
            case ItemType.ONE_MISS_DEFENCE:
                _descText.SetText(LocalizationTextKey.STORE_ONEMISSDEFFENCE_DESC);
                break;
            case ItemType.BONUS_SCORE:
                _descText.SetText(LocalizationTextKey.STORE_BONUSSCORE_DESC);
                break;
            case ItemType.HINT:
                _descText.SetText(LocalizationTextKey.STORE_HINT_DESC);
                break;
            case ItemType.FREEZE_TIME:
                _descText.SetText(LocalizationTextKey.STORE_FREEZETIME_DESC);
                break;
            case ItemType.EXTRA_TIME:
                _descText.SetText(LocalizationTextKey.STORE_EXTRATIME_DESC);
                break;
            default:
                break;
        }

    }

    override public List<ShopInfo> GetProductList()
    {
       return DataManager.Instance.GetShopInfoList(_itemType);
    }

    override public void Show(BaseStateData baseStateData)
    {
        ItemShopStateData itemShopStateDataStateData = (ItemShopStateData)baseStateData;
        Show(itemShopStateDataStateData.itemType == _itemType);
    }

    override public void OnTab()
    {
        _shopState.OnTab(_itemShopStateData);
    }
}
