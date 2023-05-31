using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneContext<T> : MonoBehaviour where T : SceneContext<T>
{
    #region Fields

    private static T instance;
    private static bool shuttingDown = false;
    private static object lockObj = new object();

    #endregion

    #region Properties

    public static T Instance
    {
        get
        {
            if (!Application.isPlaying) return null;
            
            //종료 시에 Object보다 싱글톤의 OnDestroy가 먼저 실행 될 수도 있기 때문에 처리
            if(shuttingDown)
            {
                Logger.Log("[SingleTon] Instance '" 
                           + typeof(T) 
                           + "' already destroyed. Returning null."
                    , Logger.LogLevel.Warning);
                return null;
            }
                
            //쓰레드 Safe
            lock (lockObj)
            {
                //이미 생성되어 있으면 그대로 반환
                if (instance != null) return instance;
                
                //싱글톤 인스턴스 찾기
                instance = (T)FindObjectOfType(typeof(T), true);
                        
                //찾았는데 있으면 반환
                if (instance != null) return instance;
                    
                var go = new GameObject();
                instance = go.AddComponent<T>();
                go.name = $"@{typeof(T)}";

                return instance;
            }
        }
    }

    #endregion

    #region Unity Lifecycle

    protected virtual void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            Logger.LogError($"{typeof(T)} : {GetInstanceID()} 중복 생성된 씬 오브젝트 객체가 있어 파괴됩니다.");
        }
        else Logger.LogEnter<T>();
    }

    #endregion

}
