[System.Serializable]
public class StageInfo {

    public string themeId;
    public string stageId;
    public string name;
    public RewardInfo rewardBasic;
    public RewardInfo rewardMaster;

    public int stageNo;
   
    public override string ToString()
    {
        return "\n themeId : " + themeId
            + "\n stageId : " + stageId
            + "\n name : " + name      
            + "\n rewardBasic : " + rewardBasic.ToString()
        +"\n rewardMaster : " + rewardMaster.ToString()
             ;
    }

}
