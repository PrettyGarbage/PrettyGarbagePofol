using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class ShopStateData : BaseStateData
{
    public ShopType shopType;
    public ShopMainType shopMainType;

    public ShopStateData() { }
    public ShopStateData(ShopType shopType, ShopMainType shopMainType)
    {
        this.shopType = shopType;
        this.shopMainType = shopMainType;
    }
}

public class ShopState : PopupState
{
    protected IShopList[] shopListCtrls;

    private bool _isOpenItemShop;

    public static void Open(ShopMainType shopMainType)
    {
        StateManager.Instance.OpenPopupState(typeof(ShopState), new ShopStateData(ShopType.MAIN, shopMainType));
    }

    public override IEnumerator OnPreOpen<T>(T args = default(T))
    {
        yield return base.OnPreOpen(args);
        SetShopList();
        _isOpenItemShop = false;
    }

    protected virtual void SetShopList()
    {
        ShopStateData shopStateData = GetData<ShopStateData>();
        SetShopProductList();
        OnTab(shopStateData);
    }

    protected void SetShopProductList()
    {
        if (shopListCtrls == null)
        {
            shopListCtrls = GetComponentsInChildren<IShopList>();

            for (int i = 0; i < shopListCtrls.Length; i++)
            {
                shopListCtrls[i].SetProductList(this);
            }
        }
    }

    public void OnTab(BaseStateData shopStateData)
    {
        for (int i = 0; i < shopListCtrls.Length; i++)
        {
            shopListCtrls[i].Show(shopStateData);
        }
    }

    public void OnOpenItemShop()
    {
        OnBack();
        _isOpenItemShop = true;
    }

    public override void OnPostClosed()
    {
        if(_isOpenItemShop)
            ItemShopState.Open(ItemType.HINT);
    }

}