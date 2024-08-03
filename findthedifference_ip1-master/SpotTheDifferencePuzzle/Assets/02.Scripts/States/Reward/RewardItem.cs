using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardItem : MonoBehaviour {

    [SerializeField]
    private LocalizationUIText _rewardName;
    [SerializeField]
    private Image _itemImage;
    [SerializeField]
    private TMP_Text _amountText;

    public void SetRewardItem(ProductItemInfo productItemInfo)
    {
        BaseSpriteData baseSpriteData = ResourceDataManager.Instance.GetBaseSpriteData(productItemInfo);
        if (baseSpriteData != null)
        {
            SetItem(baseSpriteData.LocalizedTextKey, baseSpriteData.Sprite, productItemInfo.TotalCount);
        }
        else
        {
            if(productItemInfo.type == ProductType.COLLECTION)
            {
                SetItem(LocalizationTextKey.COLLECTION_MAIN_TITLE.ToString(), null, 1);
            }
        }
    }

    private void SetItem(string localizedTextKey, Sprite sprite, int amount)
    {
        _itemImage.sprite = sprite;
        _rewardName.SetText(localizedTextKey);
        _amountText.text = SystemUtil.GetCommaText(amount);
    }

	
}
