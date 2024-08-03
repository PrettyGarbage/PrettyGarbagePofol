using System.Collections.Generic;
using System.Text;

[System.Serializable]
public class ProductInfo
{
    public string id;
    public string productItemId;
    public string name;
    public float price;
    public int buyLimitCount;
    public string androidId;
    public string iosId;
    public PaymentType paymentType;
    public List<ProductItemInfo> productItemList;

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("\n id : " + id
            + "\n productItemId : " + productItemId
            + "\n name : " + name
            + "\n name : " + name
            + "\n price : " + price
            + "\n buyLimitCount : " + buyLimitCount
            + "\n androidId : " + androidId
            + "\n iosId : " + iosId
            + "\n paymentType : " + paymentType
            );

        for (int i = 0; i < productItemList.Count; i++)
        {
            sb.AppendLine(productItemList[i].ToString());
        }

        return sb.ToString();

    }

    public ApiErrorCode GetBuyEnableCode()
    {
        ApiErrorCode apiErrorCode = ApiErrorCode.NONE;
        switch (paymentType)
        {
            case PaymentType.GOLD:
                if (DataManager.Instance.UserInfo.Gold < price)
                {
                    apiErrorCode = ApiErrorCode.NOT_ENOUGH_GOLD;
                }
                break;
            case PaymentType.STARCOIN:
                if (DataManager.Instance.UserInfo.StarCoin < price)
                {
                    apiErrorCode = ApiErrorCode.NOT_ENOUGH_STARCOIN;
                }
                break;
            case PaymentType.CASH:
            default:
                break;
        }
        return apiErrorCode;
    }

    public string GetStoreProductId()
    {
#if UNITY_ANDROID
        return androidId;
#elif UNITY_IOS
        return iosId;        
#endif
    }

}