[System.Serializable]
public class UserStageInfo
{
    public string stageId;
    public UserStageDetailInfo stageDetailBasic;
    public UserStageDetailInfo stageDetailMaster;

    public override string ToString()
    {
        return "\n stageId : " + stageId
            + "\n stageDetailBasic : " + stageDetailBasic.ToString()
            + "\n stageDetailMaster : " + stageDetailMaster.ToString()
             ;
    }

    public UserStageDetailInfo GetUserStageDetailInfo(ThemeDifficulty themeDifficulty)
    {
        if(themeDifficulty == ThemeDifficulty.THEME_BASIC)
        {
            return stageDetailBasic;
        }else
        {
            return stageDetailMaster;
        }
    }

}
