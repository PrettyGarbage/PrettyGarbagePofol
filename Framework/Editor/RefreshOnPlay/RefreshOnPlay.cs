using UnityEditor;
using UnityEngine;
using Debug = Mine.Code.Framework.Util.Debug.Debug;

namespace Mine.Code.Framework.Editor.RefreshOnPlay
{
    [InitializeOnLoad]

    public static class RefreshOnPlay
    {

        static RefreshOnPlay()
        {
            EditorApplication.playModeStateChanged += PlayRefresh;
        }

        static void PlayRefresh(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode && !EditorPrefs.GetBool("kAutoRefresh"))
            {
                Util.Debug.Debug.Log("Refresh on play..");
                AssetDatabase.Refresh();
            }
        }
    }
}