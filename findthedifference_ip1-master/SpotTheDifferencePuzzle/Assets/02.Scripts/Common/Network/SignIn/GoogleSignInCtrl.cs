using Google;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GoogleSignInCtrl
{
    private GoogleSignInConfiguration _configuration;
    Action<GoogleSignInUser> _onSigninCallback;

    public GoogleSignInCtrl()
    {
        Debug.Log(" GoogleWebClientId : " + BuildManager.Instance.BuildInfoData.GoogleWebClientId);

        _configuration = new GoogleSignInConfiguration
        {
            WebClientId = BuildManager.Instance.BuildInfoData.GoogleWebClientId,
            RequestIdToken = true
        };
    }

    public void SignIn(Action<GoogleSignInUser> onSigninCallback)
    {
        GoogleSignIn.Configuration = _configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
        _onSigninCallback = onSigninCallback;
    }

    public void OnSignOut()
    {
        GoogleSignIn.DefaultInstance.SignOut();
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<System.Exception> enumerator =
                    task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error =
                            (GoogleSignIn.SignInException)enumerator.Current;
                    Debug.Log("Got Error: " + error.Status + " | " + error.Message);
                }
                else
                {
                    Debug.Log("Got Unexpected Exception?!?" + task.Exception);
                }
            }
            _onSigninCallback(null);
        }
        else if (task.IsCanceled)
        {
            Debug.Log("Canceled");
            _onSigninCallback(null);
        }
        else
        {
            Debug.Log("Welcome: " + task.Result.DisplayName + "!");
            _onSigninCallback(task.Result);
        }
    }
}
