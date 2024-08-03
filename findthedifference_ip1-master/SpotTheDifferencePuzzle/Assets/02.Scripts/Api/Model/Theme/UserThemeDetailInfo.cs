[System.Serializable]
public class UserThemeDetailInfo
{

    public string themeId;
    public int obtainStarCoin;
    public int stageClearCount;


    public override string ToString()
    {
        return "\n themeId : " + themeId
            + "\n obtainStarCoin : " + obtainStarCoin
            + "\n stageClearCount : " + stageClearCount
             ;
    }

}
