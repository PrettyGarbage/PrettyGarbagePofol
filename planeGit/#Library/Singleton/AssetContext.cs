using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Library.Singleton
{
    /// <summary>
    /// 상속 받은 대상을 싱글톤 스크립터블 오브젝트로 만듭니다.
    /// </summary>
    /// <typeparam name="T">상속 받은 대상</typeparam>
    public class AssetContext<T> : ScriptableObject where T : AssetContext<T>
    {
        #region Field

#if UNITY_EDITOR
        static string filePath = $"Assets/#06.ScriptableObject/Resources/{typeof(T).Name}.asset";
#endif

        static T instance;

        #endregion

        #region Property

        public static T In
        {
            get
            {
                if (instance) return instance;

                instance = Resources.Load<T>(typeof(T).Name);

                CreateInstance();

                return instance;
            }
        }

        #endregion

        #region 에디터 전용

        public static void CreateInstance()
        {
#if UNITY_EDITOR
            if (!instance)
            {
                if (!AssetDatabase.IsValidFolder(filePath))
                {
                    var paths = filePath.Split('/');
                    for (int i = 1; i < paths.Length - 1; i++)
                    {
                        if (!AssetDatabase.IsValidFolder(string.Join("/", paths, 0, i + 1)))
                        {
                            AssetDatabase.CreateFolder(string.Join("/", paths, 0, i), paths[i]);
                        }
                    }
                }

                instance = AssetDatabase.LoadAssetAtPath<T>(filePath);

                if (!instance)
                {
                    instance = CreateInstance<T>();
                    AssetDatabase.CreateAsset(instance, filePath);
                    AssetDatabase.ImportAsset(filePath);
                }
            }
#endif
        }

        #endregion
    } 
}