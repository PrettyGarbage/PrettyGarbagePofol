[System.Serializable]
public class CollectionInfo {

    public string id;
    public string name;
    public string desc;
    public CollectionGradeType gradeType;
    public UnlockConditionInfo unlockCondition;

    public AssetBundleInfo assetBundle;

    public override string ToString()
    {
        return "\n id : " + id
            + "\n name : " + name
            + "\n desc : " + desc
            + "\n gradeType : " + gradeType
            + "\n unlockConditionInfo : " + unlockCondition.ToString()
            + "\n assetBundle : " + assetBundle.ToString()
             ;
    }

}
