[System.Serializable]
public class UserStageDetailInfo
{

    public int userMaxScore = 0;
    public int userStarCoin = 0;
    public bool claimReward = false;

    public override string ToString()
    {
        return "\n userMaxScore : " + userMaxScore
            + "\n userStarCoin : " + userStarCoin
            + "\n isClaimReward : " + claimReward
             ;
    }

}
