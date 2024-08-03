#if UNITY_IOS

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameCenterServiceCtrl : MonoBehaviour, IMobilePlatform
{

    const string leaderboardId = "";


	public void Init()
    {
        Debug.Log("gbros GameCenterServiceCtrl Init ");
    }

	public void LogIn (ActionBool onLogIn=null)
    {
        Social.localUser.Authenticate((bool success) =>
        {
            onLogIn(success);
        });
    }

	public void ShowLeaderBoard ()
    {
        Social.ShowLeaderboardUI();
    }

	public void AddScoreToLeaderBorad (long score)
    {
        Debug.Log ("gbros AddScoreToLeaderBorad : " + score);
        if (IsLoggedIn())
        {
            Social.ReportScore(score, leaderboardId, (bool success) =>
            {
                if (success)
                {
                    Debug.Log("Update Score Success");
                }
                else
                {
                    Debug.Log("Update Score Fail");
                }
            });
        }
    }

	public void LogOut ()
    {

    }

    public bool IsLoggedIn(){
        return Social.localUser.authenticated;
    }

    public string GetUserId(){
        return IsLoggedIn() ? Social.localUser.id : string.Empty;
    }


}
#endif

