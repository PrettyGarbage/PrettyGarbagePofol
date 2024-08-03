using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSpriteData
{
    [SerializeField]
    private Sprite _sprite;
    [SerializeField]
    private string _localizedTextKey;
    public Sprite Sprite { get { return _sprite; } }
    public string LocalizedTextKey { get { return _localizedTextKey; } }
}

[System.Serializable]
public class BaseResourceData<T> : BaseSpriteData
{
    [SerializeField]
    private T _type;
    public T Type { get { return _type; } }  
}

[System.Serializable]
public class ItemSpriteData : BaseResourceData<ItemType>{}
[System.Serializable]
public class CurrencySpriteData : BaseResourceData<CurrencyType> { }
[System.Serializable]
public class PaymentSpriteData : BaseResourceData<PaymentType> { }
[System.Serializable]
public class EventShopSpriteData : BaseResourceData<string> { }

public class ResourceDataManager : MonoBehaviour {

    private static ResourceDataManager _instance;
    public static ResourceDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = ObjectUtil.CreateInstance<ResourceDataManager>("ResourceDataManager");
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    [SerializeField]
    private List<ItemSpriteData> _itemSpriteDatas;
    [SerializeField]
    private List<CurrencySpriteData> _currencySpriteDatas;
    [SerializeField]
    private List<PaymentSpriteData> _paymentSpriteDatas;
    [SerializeField]
    private List<EventShopSpriteData> _eventShopSpriteData;

    public ItemSpriteData GetItemResourceData(ItemType itemType)
    {
        return _itemSpriteDatas.Find(i => i.Type == itemType);
    }

    public CurrencySpriteData GetCurrencyResourceData(CurrencyType itemType)
    {
        return _currencySpriteDatas.Find(i => i.Type == itemType);
    }

    public PaymentSpriteData GetPaymentResourceData(PaymentType paymentType)
    {
        return _paymentSpriteDatas.Find(i => i.Type == paymentType);
    }

    public EventShopSpriteData GetEventShopSpriteData(string eventShopName)
    {
        return _eventShopSpriteData.Find(i => i.Type == eventShopName);
    }

    public List<BaseSpriteData> GetSpriteDataList(ProductInfo productInfo)
    {
        List<BaseSpriteData> baseSpriteDataList = new List<BaseSpriteData>();
        for (int i = 0; i < productInfo.productItemList.Count; i++)
        {
            baseSpriteDataList.Add(GetBaseSpriteData(productInfo.productItemList[i]));
        }

        return baseSpriteDataList;
    }

    public BaseSpriteData GetBaseSpriteData(ProductItemInfo productItemInfo)
    {
        switch (productItemInfo.type)
        {
            case ProductType.ITEM:
                ItemInfo itemInfo = DataManager.Instance.GetItemInfo(productItemInfo.itemId);
                return ResourceDataManager.Instance.GetItemResourceData(itemInfo.type);                
            case ProductType.CURRENCY:
                return ResourceDataManager.Instance.GetCurrencyResourceData(productItemInfo.currencyType);
            case ProductType.COLLECTION:
            default:
                return null;
        }
    }
}
