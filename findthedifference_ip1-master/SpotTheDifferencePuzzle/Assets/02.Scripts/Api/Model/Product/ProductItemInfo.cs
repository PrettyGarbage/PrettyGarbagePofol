[System.Serializable]
public class ProductItemInfo
{
    public string id;
    public string name;
    public int seq;
    public ProductType type;
    public int count;
    public int bonusCount;
    public string itemId;
    public string collectionId;
    public CurrencyType currencyType;


    public int TotalCount
    {
        get
        {
            return count + bonusCount;
        }
    }

    public override string ToString()
    {
        return "\n id : " + id
            + "\n seq : " + seq
            + "\n name : " + name
            + "\n ProductType : " + type
            + "\n count : " + count
            + "\n bonusCount : " + bonusCount
            + "\n itemId : " + itemId
            + "\n collectionId : " + collectionId
            + "\n currencyType : " + currencyType
             ;
    }
}