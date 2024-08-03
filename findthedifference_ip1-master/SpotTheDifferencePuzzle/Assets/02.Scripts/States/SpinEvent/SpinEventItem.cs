using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpinEventItem : MonoBehaviour {

    [SerializeField]
    private string _id;

    [SerializeField]
    private ProductType _productionType;
    [SerializeField]
    private ItemType _itemType;
    [SerializeField]
    private CurrencyType _currencyType;
    [SerializeField]
    private Image _itemImage;

    [SerializeField]
    private LocalizationUIText _name;

    public string Id { get { return _id; } }

    
    public void Initialize()
    {
        if (!_id.Equals("jackpot"))
        {

            switch (_productionType)
            {
                case ProductType.ITEM:
                    ItemSpriteData itemSpriteData = ResourceDataManager.Instance.GetItemResourceData(_itemType);
                    _itemImage.sprite = itemSpriteData.Sprite;
                    _name.SetText(itemSpriteData.LocalizedTextKey);
                    break;
                case ProductType.CURRENCY:
                    CurrencySpriteData currencySpriteData = ResourceDataManager.Instance.GetCurrencyResourceData(_currencyType);
                    _itemImage.sprite = currencySpriteData.Sprite;
                    _name.SetText(currencySpriteData.LocalizedTextKey);
                    break;
                case ProductType.COLLECTION:                    
                default:
                    break;
            }

            
            
        }
    }


}
