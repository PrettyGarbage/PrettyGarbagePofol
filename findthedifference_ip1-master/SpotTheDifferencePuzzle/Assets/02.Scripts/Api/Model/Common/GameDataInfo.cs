using System.Collections.Generic;

[System.Serializable]
public class GameDataInfo  {
    public List<ThemeInfo> themeList;
    public List<ItemInfo> itemList;
    public List<StageInfo> stageList;
    public List<BaseScoreInfo> baseScoreList;
    public List<ShopInfo> shopList;
    public List<ProductInfo> productList;
    public List<CollectionInfo> collectiontList;
    public DailyRewardInfo dailyReward;
    public List<string> limitProductIdList;
    public List<RewardAdInfo> rewardAdList;
}
