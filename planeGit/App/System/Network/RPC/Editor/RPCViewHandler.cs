using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Library.Network
{
    [InitializeOnLoad]
    public class RPCViewHandler : EditorWindow
    {
        static RPCViewHandler()
        {
            // called once per change (per key-press in inspectors) and once after play-mode ends.
#if (UNITY_2018 || UNITY_2018_1_OR_NEWER)
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
#else
		    EditorApplication.hierarchyWindowChanged += OnHierarchyChanged;
#endif
        }

        internal static void OnHierarchyChanged()
        {
            if (Application.isPlaying) return;

            RPCView[] networkViewResources = Resources.FindObjectsOfTypeAll<RPCView>();

            if(networkViewResources.All(view => IsPrefab(view.gameObject))) return;
            
            int targetViewId = networkViewResources.FirstOrDefault(view => !IsPrefab(view.gameObject))!.gameObject.scene.buildIndex * 1000 + 1;
            
            foreach (RPCView view in networkViewResources)
            {
                if (IsPrefab(view.gameObject))
                {
                    // prefabs should use 0 as ViewID and sceneViewId
                    if (view.ViewID != 0 || view.SceneViewID != 0)
                    {
                        view.ViewID = 0;
                        view.SceneViewID = 0;
                        EditorUtility.SetDirty(view);
                    }

                    continue;   // skip prefabs in further processing
                }

                if (targetViewId > (networkViewResources.FirstOrDefault(view => !IsPrefab(view.gameObject))!.gameObject.scene.buildIndex * 1000) + 1000)
                {
                    Logger.LogError("허용된 NetworkView 개수를 넘었습니다.");
                    continue;
                }

                if (view.SceneViewID != targetViewId)
                {
                    view.SceneViewID = targetViewId;
                    EditorUtility.SetDirty(view);
                }

                targetViewId++;
            }
        }

        static bool IsPrefab(GameObject go)
        {
#if UNITY_2021_2_OR_NEWER
            return UnityEditor.SceneManagement.PrefabStageUtility.GetPrefabStage(go) != null || EditorUtility.IsPersistent(go);
#elif UNITY_2018_3_OR_NEWER
            return UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetPrefabStage(go) != null || EditorUtility.IsPersistent(go);
#else
            return EditorUtility.IsPersistent(go);
#endif
        }
    }
}