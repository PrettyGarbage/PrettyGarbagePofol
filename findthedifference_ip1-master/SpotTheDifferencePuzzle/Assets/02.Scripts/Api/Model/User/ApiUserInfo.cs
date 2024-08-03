using System.Collections.Generic;
using System.Text;

[System.Serializable]
public class ApiUserInfo {
    public int id;
    public string name;
    public string platformId;
    public int paidGold;
    public int freeGold;
    public int starCoin;
    public LoginType loginType;

    //jp
    public string locale;
    public string birthDay;

    //Heart
    public int heart;
    public long lastGetheartTimestamp;
    public int remainHeartChargeSeconds;
    public int remainUnLimitHeartSeconds;

    public List<UserItemInfo> itemList;
    public List<UserCollectionInfo> collectionList;
    
    public long lastLoginTimestamp;

    public override string ToString()
    {
        StringBuilder itemListSB = new StringBuilder();
        foreach (UserItemInfo item in itemList)
        {
            itemListSB.AppendLine(item.ToString());
        }

        StringBuilder collectionListSB = new StringBuilder();
        foreach (UserCollectionInfo item in collectionList)
        {
            collectionListSB.AppendLine(item.ToString());
        }

        return "id : " + id
            + "\n name : " + name
            + "\n loginType : " + loginType
            + "\n platformId : " + platformId
            + "\n paidGold : " + paidGold
            + "\n freeGold : " + freeGold
            + "\n starCoin : " + starCoin
            + "\n heart : " + heart
            + "\n locale : " + locale
            + "\n birthDay : " + birthDay
            + "\n lastGetheartTimestamp : " + lastGetheartTimestamp
            + "\n remainHeartChargeSeconds : " + remainHeartChargeSeconds
            + "\n remainUnLimitHeartSeconds : " + remainUnLimitHeartSeconds
            + "\n lastLoginTimestamp : " + lastLoginTimestamp
            + "\n itemList : " + itemListSB.ToString()

            ;
    }
}
