using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	#region  SINGLETON
	static GameManager _instance;

    private InGameData _inGameData;
    public InGameData InGameData
    {
        get { return _inGameData; }
    }

    public static GameManager Instance
	{
		get
		{
			return _instance;
		}
	}
    #endregion SINGLETON

	public int themeRoundCompltedCount = 0;

    //User's Infos
    public List<UserThemeInfo> userThemeInfoList;
    
    #region UNITY EVENTS
    void Awake()
	{
        #region SINGLETON
        if (GameManager.Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            Initialize();
        }
#endregion SINGLETON
    }
    #endregion UNITY EVENTS

    void Initialize()
	{
        _inGameData = new InGameData();
    }

    public void Restart()
    {
        SoundManager.Instance.StopAllBgm();
        SceneManager.LoadScene(0);
    }

    public string GetThemeDifficutyText(ThemeDifficulty themeDifficulty)
    {
        return themeDifficulty == ThemeDifficulty.THEME_BASIC
            ? LocalizationManager.Instance.GetText(LocalizationTextKey.COMMON_DIFFICULTY_NORMAL) 
            : LocalizationManager.Instance.GetText(LocalizationTextKey.COMMON_DIFFICULTY_MASTER);
    }

    public LocalizationTextKey GetThemeDifficutyTextKey(ThemeDifficulty themeDifficulty)
    {
        return themeDifficulty == ThemeDifficulty.THEME_BASIC
            ? LocalizationTextKey.COMMON_DIFFICULTY_NORMAL
            : LocalizationTextKey.COMMON_DIFFICULTY_MASTER;
    }

    public string GetThemeNameKey(string themeId)
    {
        return GameConstants.TEXT_THEMENAME + themeId.ToUpper();
    }

    public IEnumerator GetUserStageInfo()
    {
        bool isLoaded = false;

        ApiManager.Instance.GetUserStageList(GameManager.Instance.InGameData.ThemeInfo.id, (apiResult) =>
        {
            GameManager.Instance.InGameData.SetUserStageInfoList(apiResult.result);

            isLoaded = true;
        });

        while (!isLoaded)
        {
            yield return CommonConstants.WaitLoopSeconds;
        }
    }
}