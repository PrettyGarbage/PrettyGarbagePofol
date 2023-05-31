using UnityEngine;
using UnityEditor;

public static class PrefabUtilityExtensions
{
    /*public static GameObject FindPrefabByName(string prefabName)
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab " + prefabName);
        if (guids.Length == 0)
        {
            Debug.LogError("Prefab not found.");
            return null;
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

        if (prefab == null)
        {
            Debug.LogError("Failed to load prefab.");
            return null;
        }

        return prefab;
    }*/
}