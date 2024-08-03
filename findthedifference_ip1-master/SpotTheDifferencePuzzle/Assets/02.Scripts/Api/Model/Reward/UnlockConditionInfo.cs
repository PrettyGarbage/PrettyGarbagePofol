[System.Serializable]
public class UnlockConditionInfo
{

    public string id;
    public UnlockConditionType type;
    public string typeId;

    public override string ToString()
    {
        return "\n id : " + id
            + "\n UnlockConditionType : " + type
            + "\n typeId : " + typeId
             ;
    }

}
