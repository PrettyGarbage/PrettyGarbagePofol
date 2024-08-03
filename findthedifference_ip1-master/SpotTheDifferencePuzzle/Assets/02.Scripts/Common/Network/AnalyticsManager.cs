using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
//using Firebase;
//using Firebase.Analytics;
using UnityEngine;

public class AnalyticsEventParam{

	private Dictionary<string, object> _paramDic;

    public AnalyticsEventParam()
    {
		_paramDic = new Dictionary<string, object>();
    }

    public Dictionary<string, object> ParamDic
    {
        get
        {
            return _paramDic;
        }
    }

    public AnalyticsEventParam AddParam(string paramName, string paramValue){
		_paramDic[paramName] = paramValue;
		return this;
	}
	public AnalyticsEventParam AddParam(string paramName, int paramValue){
		_paramDic[paramName] = paramValue;
		return this;
	}
	public AnalyticsEventParam AddParam(string paramName, float paramValue){
		_paramDic[paramName] = paramValue;
		return this;
	}
}

public class AnalyticsManager : MonoBehaviour {

	private static AnalyticsManager _instance;

	//private FirebaseApp _app;
	public static AnalyticsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = ObjectUtil.CreateInstance<AnalyticsManager>("AnalyticsManager");
                DontDestroyOnLoad(_instance.gameObject);
                
            }

            return _instance;
        }
    }

    public void Init()
    {
        //if(_app!=null)
        //    return;

#if UNITY_EDITOR
        return;
#endif

        Debug.Log("AnalyticsManager Init");

        //Firebase.FirebaseApp.CheckAndFixDependenciesAsync();

        //Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        //{
        //    Debug.Log("task : " + task.Result);

        //    var dependencyStatus = task.Result;
        //    Debug.Log("gbros CheckAndFixDependenciesAsync : " + dependencyStatus.ToString());
        //    if (dependencyStatus == Firebase.DependencyStatus.Available)
        //    {
        //        // Create and hold a reference to your FirebaseApp, i.e.
        //        _app = Firebase.FirebaseApp.DefaultInstance;

        //        // where app is a Firebase.FirebaseApp property of your application class.

        //        // Set a flag here indicating that Firebase is ready to use by your
        //        // application.
        //    }
        //    else
        //    {
        //        UnityEngine.Debug.LogError(System.String.Format(
        //          "gbros Could not resolve all Firebase dependencies: {0}", dependencyStatus));
        //        // Firebase Unity SDK is not safe to use here.
        //    }
        //});

    }

    public void EventLog(string eventName){
        // Log an event with an int parameter.
        //if (_app != null)
        //{
        //    Debug.Log("gbros EventLog : " + eventName);
        //    FirebaseAnalytics.LogEvent(eventName);
        //}
		
    }
	
	public void EventLog(string eventName, AnalyticsEventParam analyticsEventParam){
        //if (_app != null)
        //{
        //    // Log an event with an int parameter.
        //    if (eventName.Length > 0 && analyticsEventParam.ParamDic.Count > 0)
        //    {
        //        Parameter[] LevelUpParameters = new Parameter[analyticsEventParam.ParamDic.Count];
        //        int index = 0;
        //        foreach (string paramName in analyticsEventParam.ParamDic.Keys)
        //        {
        //            object paramValueObject = analyticsEventParam.ParamDic[paramName];
        //            if (paramValueObject is int)
        //            {
        //                LevelUpParameters[index] = new Firebase.Analytics.Parameter(paramName, (int)paramValueObject);
        //                index++;
        //            }
        //            else if (paramValueObject is float)
        //            {
        //                LevelUpParameters[index] = new Firebase.Analytics.Parameter(paramName, (float)paramValueObject);
        //                index++;
        //            }
        //            else if (paramValueObject is string)
        //            {
        //                LevelUpParameters[index] = new Firebase.Analytics.Parameter(paramName, (string)paramValueObject);
        //                index++;
        //            }
        //        }

        //        Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName, LevelUpParameters);
        //    }
        //}
        
    }
}
