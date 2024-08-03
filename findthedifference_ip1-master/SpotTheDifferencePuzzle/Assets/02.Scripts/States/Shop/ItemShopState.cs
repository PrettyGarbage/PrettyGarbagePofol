using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemShopStateData : BaseStateData
{
    public ShopType shopType;
    public ItemType itemType;

    public ItemShopStateData() { }
    public ItemShopStateData(ShopType shopType, ItemType itemType)
    {
        this.shopType = shopType;
        this.itemType = itemType;
    }
}

public class ItemShopState : ShopState
{

    public static void Open(ItemType itemType)
    {
        StateManager.Instance.OpenPopupState(typeof(ItemShopState), new ItemShopStateData(ShopType.ITEM, itemType));
    }

    public override IEnumerator OnPreOpen<T>(T args = default(T))
    {
        yield return base.OnPreOpen(args);       
    }

    protected override void SetShopList()
    {
        ItemShopStateData shopStateData = GetData<ItemShopStateData>();
        SetShopProductList();
        OnTab(shopStateData);
    }
}
