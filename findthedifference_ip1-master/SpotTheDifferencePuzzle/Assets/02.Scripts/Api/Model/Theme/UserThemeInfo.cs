[System.Serializable]
public class UserThemeInfo  {

    public int userId;
    public string themeId;
    public int unlockYn;
    public ThemeDifficulty themeDifficulty;
    public UserThemeDetailInfo themeDetailBasic;
    public UserThemeDetailInfo themeDetailMaster;

    public override string ToString()
    {
        return "\n userId : " + userId
            + "\n themeId : " + themeId
            + "\n unlockYn : " + unlockYn
            + "\n ThemeDifficulty : " + themeDifficulty
            + "\n themeDetailBasic : " + themeDetailBasic.ToString()
            + "\n themeDetailMaster : " + themeDetailMaster.ToString()
             ;
    }

}
