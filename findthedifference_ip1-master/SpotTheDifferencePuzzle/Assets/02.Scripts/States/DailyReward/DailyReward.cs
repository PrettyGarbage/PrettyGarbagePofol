using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class DailyReward : MonoBehaviour {

    [SerializeField]
    private LocalizationUIText _titleText;
    [SerializeField]
    private Image _clearImage;
    [SerializeField]
    private Image _clearImageBg;
    [SerializeField]
    private RewardItem _rewardItem;

    private List<ProductItemInfo> _productItemInfos;

    public void SetDailyReward(RewardInfo rewardInfo, int day, bool isRewarded, bool isCurrentReward)
    {
        Debug.Log(string.Format("day : {0}, isReward : {1}, isCurrentReward : {2}", day, isRewarded, isCurrentReward));
        _titleText.SetText(string.Format(GameConstants.TEXT_DAILY_REWARD_DAY, day));
        _clearImage.gameObject.SetActive(isRewarded);
        _clearImageBg.gameObject.SetActive(isRewarded);

        SetRewardItem(rewardInfo.productItemList);

        if (isCurrentReward)
        {
            StartCoroutine( RewardAction() );
        }

    }

    private void SetRewardItem(List<ProductItemInfo> productItemInfos)
    {
        if (productItemInfos.Count > 0)
        {
            _productItemInfos = productItemInfos;
            _rewardItem.SetRewardItem(productItemInfos[0]);
        }
    }

    IEnumerator RewardAction()
    {
        _clearImage.transform.parent = transform.parent.parent;
        _clearImage.transform.SetAsLastSibling();
        
        _clearImage.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.DAILY_CHECK, (isEndAfterPlay)=> {
            if (isEndAfterPlay)
            {
                SoundManager.Instance.PlayVoiceSound(RandomVoiceConstants.CALL_NAME);
            }
        });

        _clearImage.transform.position = transform.position;

        _clearImage.gameObject.SetActive(true);
        _clearImage.transform.localScale = Vector3.one * 3f;
        _clearImage.color = GameConstants.TRANSPARENT;        
        LeanTween.value(0f, 1f, 1f).setOnUpdate((float v) => {
            _clearImage.color = GameConstants.OPAQUE * v;
        }).setOnComplete(()=> _clearImageBg.gameObject.SetActive(true));
        LeanTween.scale(_clearImage.gameObject, Vector3.one, 1f)
            .setEaseInElastic()
            .setOnComplete(() =>
            {
                SoundManager.Instance.PlayUISound(gameObject, AudioDataKey.ui_event_dailycheck_click);
                PopupRewardState.Open(_productItemInfos);
            });
    }


}
