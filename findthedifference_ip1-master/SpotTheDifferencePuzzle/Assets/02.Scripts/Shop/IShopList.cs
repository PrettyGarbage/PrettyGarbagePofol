using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShopList {

    void SetProductList(ShopState shopState);
    List<ShopInfo> GetProductList();
    void Show(BaseStateData baseStateData);
    void OnTab();
}
