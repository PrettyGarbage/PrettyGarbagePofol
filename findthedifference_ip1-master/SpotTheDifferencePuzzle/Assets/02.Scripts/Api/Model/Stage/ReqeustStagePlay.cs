using System.Collections.Generic;

[System.Serializable]
public class ReqeustStagePlay {

    public int userId;
    public string themeId;
    public string stageId;
    public ThemeDifficulty difficultyType;
    public List<string> itemIdList;

}
