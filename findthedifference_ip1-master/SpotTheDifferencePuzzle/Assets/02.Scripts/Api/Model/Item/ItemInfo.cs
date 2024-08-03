[System.Serializable]
public class ItemInfo  {

    public string id;
    public string name;
    public string desc;
    public ItemType type;
    public ItemUseType useType;

    public override string ToString()
    {
        return "\n id : " + id
            + "\n name : " + name
            + "\n type : " + type
            + "\n useType : " + useType
            + "\n desc : " + desc
             ;
    }
}
