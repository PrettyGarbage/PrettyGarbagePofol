using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstEventProduct : MonoBehaviour {

    [Header("event product")]
    [SerializeField]
    private EventProductCtrl _pfEventProductCtrl;
    private EventProductCtrl _eventProductCtrl;

    private void OnEnable()
    {
        CreateEventProduct();
    }

    public void CreateEventProduct()
    {
        ShopInfo shopInfo = DataManager.Instance.GetFirstEventProduct();
        if (shopInfo != null)
        {
            if (_eventProductCtrl == null)
            {
                _eventProductCtrl = Instantiate<EventProductCtrl>(_pfEventProductCtrl, transform.position, Quaternion.identity, transform);
                _eventProductCtrl.transform.localPosition = Vector3.zero;
            }
            ProductInfo productInfo = DataManager.Instance.GetProductInfo(shopInfo.productId);
            _eventProductCtrl.SetProductInfo(shopInfo, productInfo, null);
        }
        
    }

}
