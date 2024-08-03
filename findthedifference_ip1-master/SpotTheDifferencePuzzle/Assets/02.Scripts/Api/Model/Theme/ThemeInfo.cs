using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ThemeInfo {

    public string id;
    public string name;
    public int sortOrder;
    public string unlock_condition_id;
    public AssetBundleInfo assetBundleAndroid;
    public AssetBundleInfo assetBundleIos;
    public UnlockConditionInfo unlockCondition;
    public RewardInfo rewardBasic;
    public RewardInfo rewardMaster;

    public int stageCount;

    private Sprite _themeMainImage;
    private Dictionary<int, PuzzleImageData> _puzzleImageDataDic;

    public Sprite ThemeMainImage
    {
        get
        {
            return _themeMainImage;
        }

        set
        {
            _themeMainImage = value;
        }
    }

    public PuzzleImageData GetPuzzleImageData(int index)
    {
        if (_puzzleImageDataDic == null)
        {
            _puzzleImageDataDic = new Dictionary<int, PuzzleImageData>();
        }

        if (_puzzleImageDataDic.ContainsKey(index))
        {
            return _puzzleImageDataDic[index];
        }
        else
        {
            PuzzleImageData puzzleImageData = AssetBundleManager.Instance.GetAsset<PuzzleImageData>(id, id + GameConstants.ASSETBUNDLE_THEME_PUZZLE + (index + 1));
            _puzzleImageDataDic.Add(index, puzzleImageData);
            return puzzleImageData;
        }
    }

    public override string ToString()
    {
        return "\n id : " + id
            + "\n name : " + name
            + "\n sortOrder : " + sortOrder
            + "\n unlock_condition_id : " + unlock_condition_id
            + "\n assetBundle : " + assetBundleAndroid.ToString()
            + "\n assetBundle : " + assetBundleIos.ToString()
            + "\n unlockCondition : " + unlockCondition.ToString()
            + "\n rewardBasic : " + rewardBasic.ToString()
            + "\n rewardMaster : " + rewardMaster.ToString()
            + "\n stageCount : " + stageCount.ToString()
             ;
    }

}
