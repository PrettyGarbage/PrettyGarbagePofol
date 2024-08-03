using System.Collections.Generic;

[System.Serializable]
public class BaseScoreInfo
{
    public ThemeDifficulty themeDifficulty;

    public int starcoin1;
    public int starcoin2;
    public int starcoin3;
    public int starcoin4;
    public int starcoin5;
    public int basicScore;

    //남은 시간 대비 추가 점수
    public int timeBonusPerseconds;
    //보너스스코어 아이템 사용시 추가 곱.
    public int bonusScoreItemPercent;

    public int comboMultiple1;
    public int comboMultiple2;
    public int comboMultiple3;
    public int comboMultiple4;


}