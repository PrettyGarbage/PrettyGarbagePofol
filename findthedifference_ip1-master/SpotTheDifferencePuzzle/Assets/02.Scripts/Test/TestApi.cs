using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestEventInfo
{
    public string title;
    public UnityAction unityAction;


    public TestEventInfo(string title, UnityAction unityAction)
    {
        this.title = title;
        this.unityAction = unityAction;
    }
}

public class TestApi : MonoBehaviour
{

    private string _id = "thkim";

    [SerializeField]
    private GameObject _gridLayoutObject;
    [SerializeField]
    private TestEventButton _pfTestEventButton;

    private List<TestEventInfo> _testEventInfoList;

    private void Start()
    {
        _pfTestEventButton.gameObject.SetActive(false);

        _testEventInfoList = new List<TestEventInfo>();
        _testEventInfoList.Add(new TestEventInfo("Login", Login));
        _testEventInfoList.Add(new TestEventInfo("GetThemeList", GetThemeList));
        _testEventInfoList.Add(new TestEventInfo("GetUserInfo", GetUserInfo));
        _testEventInfoList.Add(new TestEventInfo("GetGameData", GetGameData));
        _testEventInfoList.Add(new TestEventInfo("GetUserStageList", GetUserStageList));

        _testEventInfoList.Add(new TestEventInfo("RequestPlayInfo", RequestPlayInfo));
        _testEventInfoList.Add(new TestEventInfo("SendPlayResult", SendPlayResult));
        _testEventInfoList.Add(new TestEventInfo("UseIngameItem", UseIngameItem));

        _testEventInfoList.Add(new TestEventInfo("BuyItem", BuyItem));
        _testEventInfoList.Add(new TestEventInfo("BuyGold", BuyGold));
        _testEventInfoList.Add(new TestEventInfo("BuyTimeBonus", BuyTimeBonus));
        _testEventInfoList.Add(new TestEventInfo("GetLuckySpinResult", GetLuckySpinResult));

        for (int i = 0; i < _testEventInfoList.Count; i++)
        {
            TestEventInfo testEventInfo = _testEventInfoList[i];

            TestEventButton testEventButton = Instantiate<TestEventButton>(_pfTestEventButton, _gridLayoutObject.transform);
            testEventButton.SetEvent(testEventInfo);

        }
    }

    public void Login()
    {
        Debug.Log("UserInfo.IsLogined : " + UserInfo.IsLogined());
        if (!UserInfo.IsLogined())
        {
            AuthLoginForm authLoginForm = new AuthLoginForm();
            authLoginForm.loginType = LoginType.NONE;
            authLoginForm.name = "thkimname";
            authLoginForm.id = _id;
            authLoginForm.authData = "1111";
            ApiManager.Instance.AuthLogin(authLoginForm, (apiResult) =>
            {
                Debug.Log("Login : " + apiResult.isSuccess);
            });
        }
    }

    public void GetThemeList()
    {
        Debug.Log("UserInfo.IsLogined : " + UserInfo.IsLogined());
        if (UserInfo.IsLogined())
        {
            ApiManager.Instance.GetAssetBundleData((apiResult) =>
            {
                Debug.Log(apiResult.ToString());                
            });
        }
    }

    public void GetUserInfo()
    {
        Debug.Log("UserInfo.IsLogined : " + UserInfo.IsLogined());
        if (UserInfo.IsLogined())
        {
            ApiManager.Instance.GetUserInfo((apiResult) =>
            {
                Debug.Log(apiResult.ToString());
            });
        }
    }

    public void GetUserStageList()
    {
        Debug.Log("UserInfo.IsLogined : " + UserInfo.IsLogined());
        if (UserInfo.IsLogined())
        {
            ApiManager.Instance.GetUserStageList("theme_1", (apiResult) =>
            {
                foreach (UserStageInfo stageInfo in apiResult.result)
                {
                    Debug.Log(stageInfo.ToString());
                }
            });
        }
    }

    public void GetGameData()
    {
        Debug.Log("UserInfo.IsLogined : " + UserInfo.IsLogined());
        if (UserInfo.IsLogined())
        {
            ApiManager.Instance.GetGameDataInfo((result) =>
            {
                Debug.Log(result.isSuccess);
            });
        }
    }

    public void UseIngameItem()
    {
        Debug.Log("UserInfo.IsLogined : " + UserInfo.IsLogined());
        if (UserInfo.IsLogined())
        {
            ApiManager.Instance.UseIngameItem(ItemType.HINT.ToString(), reqeustPlaySeq, apiResult =>
            {
                Debug.Log(apiResult.ToString());
            });
        }
    }

    private int reqeustPlaySeq;
    public void RequestPlayInfo()
    {
        ReqeustStagePlay reqeustStagePlay = new ReqeustStagePlay();
        reqeustStagePlay.themeId = "theme_1";
        reqeustStagePlay.stageId = "theme_1_0";
        reqeustStagePlay.difficultyType = ThemeDifficulty.THEME_BASIC;
        reqeustStagePlay.userId = 1;
        //reqeustStagePlay.itemIdList = new List<string>(){ };

        Debug.Log("UserInfo.IsLogined : " + UserInfo.IsLogined());
        if (UserInfo.IsLogined())
        {
            ApiManager.Instance.RequestPlayInfo(reqeustStagePlay, apiResult =>
            {
                Debug.Log(apiResult.ToString());
                reqeustPlaySeq = apiResult.result;
            });
        }
    }

    public void SendPlayResult()
    {
        if (reqeustPlaySeq == 0)
        {
            Debug.Log("reqeustPlaySeq is 0");
            return;
        }
        StagePlayResult stagePlayResult = new StagePlayResult();
        stagePlayResult.seq = reqeustPlaySeq;
        stagePlayResult.score = Random.Range(1000,100000);
        stagePlayResult.remainTimeSeconds = Random.Range(0, 30);
        stagePlayResult.isContinue = Random.Range(0, 30) > 15;
        
        Debug.Log("UserInfo.IsLogined : " + UserInfo.IsLogined());
        if (UserInfo.IsLogined())
        {
            ApiManager.Instance.SendPlayResult(stagePlayResult, apiResult =>
            {
                Debug.Log(apiResult.ToString());
            });
        }
    }

    public void GetLuckySpinResult()
    {
        ApiManager.Instance.GetLuckySpinResult(apiResult =>
        {
            Debug.Log(apiResult.ToString());
        });
    }

    public void BuyItem()
    {
        ApiManager.Instance.BuyByGold("itemshop", "test_hint_p", apiResult =>
        {
            Debug.Log(apiResult.ToString());
        });
    }

    public void BuyGold()
    {
        ApiManager.Instance.BuyByCash("goldshop", "gold_product_01", "",  apiResult =>
        {
            Debug.Log(apiResult.ToString());
        });
    }

    public void BuyTimeBonus()
    {
        ApiManager.Instance.BuyTimeBonus(apiResult =>
        {
            Debug.Log(apiResult.ToString());
        });
    }



}
