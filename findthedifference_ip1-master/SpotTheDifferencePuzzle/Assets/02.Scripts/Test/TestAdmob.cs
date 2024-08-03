using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.UI;

public class TestAdmob : MonoBehaviour {

    IEnumerator Start()
    {
        yield return NetworkManager.Instance.Init();

        ApiUserInfo apiUserInfo = new ApiUserInfo();
        apiUserInfo.id = 41;
        DataManager.Instance.UserInfo.SetUserData(apiUserInfo);
    }

    public void RequestBanner(){
		NetworkManager.Instance.RequestBanner(ADConstants.BANNER_INGAME, AdSize.Banner, AdPosition.Bottom);
	}

    public void RequestBannerCustom()
    {
        NetworkManager.Instance.RequestBanner(ADConstants.BANNER_INGAME, 640, 100, AdPosition.Bottom);
    }

    public void HideBanner(){
		NetworkManager.Instance.HideBanner(ADConstants.BANNER_INGAME);
	}

    public void ShowBanner()
    {
        NetworkManager.Instance.ShowBanner(ADConstants.BANNER_INGAME);
    }

    public void RemoveBanner(){
		NetworkManager.Instance.RemoveBanner(ADConstants.BANNER_INGAME);
	}

	public void ShowRewardAd(){
		NetworkManager.Instance.ShowRewardVideoAd(ADConstants.REWARD_LUCKY_SPIN, (reward)=>{

			Debug.Log("reward Type : " + reward.Type + "/ Amount :" + reward.Amount);

		}, (onClosed)=>{

			MonoBehaviour.print("gbros Closed RequestRewardVideoAd");

		});
	}


}
