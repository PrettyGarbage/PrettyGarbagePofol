[System.Serializable]
public class UserItemInfo
{
    public string itemId;
    public int count;

    public override string ToString()
    {
        return "\n itemId : " + itemId
            + "\n count : " + count
             ;
    }

}
