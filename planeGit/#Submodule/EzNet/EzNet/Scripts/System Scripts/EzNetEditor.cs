using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using EzNetLibrary;
using System.Linq;
using System;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;

[CustomEditor(typeof(EzNetHandler))]
public class EzNetHandler_Editor : Editor
{
    private SerializedProperty joinedChannels;
    private SerializedProperty channelJsonFile;
    private SerializedProperty showPacketLog;

    private void OnEnable()
    {
        joinedChannels = serializedObject.FindProperty("joinedChannels");
        channelJsonFile = serializedObject.FindProperty("channelJsonFile");
        showPacketLog = serializedObject.FindProperty("showPacketLog");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var handler = target as EzNetHandler;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Show Packet Log", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(showPacketLog, true);

        EditorGUILayout.Space();
        handler.handlerType = (EzNetHandler.HandlerType)EditorGUILayout.EnumPopup("Handler Type: ", handler.handlerType);
        switch (handler.handlerType)
        {
            case EzNetHandler.HandlerType.Client:
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("[UDP] Info", EditorStyles.boldLabel);
                handler.broadCastPort = EditorGUILayout.IntField("[UDP] Broadcast Port: ", handler.broadCastPort);

                // enum으로 Custom 또는 ChannelGroup 선택
                switch (0)
                {
                    default:
                        break;
                }
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("[TCP] My Connection Info", EditorStyles.boldLabel);
                handler.observerIP = EditorGUILayout.TextField("[TCP] Observer IP: ", handler.observerIP);
                handler.observerPort = EditorGUILayout.IntField("[TCP] Observer Port: ", handler.observerPort);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("[TCP] My Connection Info", EditorStyles.boldLabel);
                handler.opsIP = EditorGUILayout.TextField("[TCP] OPS IP: ", handler.opsIP);
                handler.opsPort = EditorGUILayout.IntField("[TCP] OPS Port: ", handler.opsPort);
                break;
            case EzNetHandler.HandlerType.Observer:
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("[UDP] Info", EditorStyles.boldLabel);
                handler.broadCastPort = EditorGUILayout.IntField("[UDP] Broadcast Port: ", handler.broadCastPort);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("[TCP] My Server Info", EditorStyles.boldLabel);
                handler.openPort = EditorGUILayout.IntField("[TCP] Open Port: ", handler.openPort);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("[TCP] My Connection Info", EditorStyles.boldLabel);
                handler.opsIP = EditorGUILayout.TextField("[TCP] OPS IP: ", handler.opsIP);
                handler.opsPort = EditorGUILayout.IntField("[TCP] OPS Port: ", handler.opsPort);
                break;
            case EzNetHandler.HandlerType.OPS:
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("[UDP] Info", EditorStyles.boldLabel);
                handler.broadCastPort = EditorGUILayout.IntField("[UDP] Broadcast Port: ", handler.broadCastPort);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("[TCP] My Server Info", EditorStyles.boldLabel);
                handler.openPort = EditorGUILayout.IntField("[TCP] Open Port: ", handler.openPort);
                break;
            default:
                break;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("[UDP, TCP] Space Info", EditorStyles.boldLabel);
        EzNetHandler.mySpace = (EzNetLibrary.Space)EditorGUILayout.EnumPopup("My Space: ", EzNetHandler.mySpace);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("[Multicast] Info", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(joinedChannels, true);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("[channelJsonFile]", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(channelJsonFile, true);

        if (GUI.changed)
        {
            serializedObject.ApplyModifiedProperties();

            EditorUtility.SetDirty(handler);
            EditorSceneManager.MarkSceneDirty(handler.gameObject.scene);
        }
    }
}


[CustomEditor(typeof(EzNetObject))]
public class EzNetObjectEditor : Editor
{

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var netObject = target as EzNetObject;

        EditorGUILayout.Space();
        netObject.objectType = (EzNetObjectType)EditorGUILayout.EnumPopup("Object Type: ", netObject.objectType);
        EditorGUILayout.Space();
        switch (netObject.objectType)
        {
            case EzNetObjectType.Dynamic:
                if (!Application.isPlaying)
                {
                    if (netObject.id != "")
                    {
                        netObject.id = "";

                        EditorUtility.SetDirty(netObject);
                        EditorSceneManager.MarkSceneDirty(netObject.gameObject.scene);
                    }
                    /*string prefabName = null;
                    GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(netObject.gameObject);
                    if (prefab != null)
                    {
                        prefabName = prefab.name;
                    }
                    else
                    {
                    }
                    if (netObject.prefabName != prefabName)
                    {
                        netObject.prefabName = prefabName;
                        serializedObject.ApplyModifiedProperties();

                        EditorUtility.SetDirty(netObject);
                        EditorSceneManager.MarkSceneDirty(netObject.gameObject.scene);
                    }*/
                }
                //EditorGUILayout.BeginHorizontal();
                //EditorGUILayout.PrefixLabel("Prefab Path");
                netObject.prefabPath = EditorGUILayout.TextField("Prefab Path: ", netObject.prefabPath);
                //EditorGUI.BeginDisabledGroup(true);
                //EditorGUILayout.TextField(netObject.prefabPath.ToString());
                //EditorGUI.EndDisabledGroup();
                //EditorGUILayout.EndHorizontal();
                //netObject.prefabName = EditorGUILayout.TextField("Prefab Name (Resources/*): ", netObject.prefabName);
                break;
            case EzNetObjectType.Static:
                break;
        }
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("ID");
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.TextField(netObject.id.ToString());
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
        if (GUI.changed)
        {
            serializedObject.ApplyModifiedProperties();

            EditorUtility.SetDirty(netObject);
            EditorSceneManager.MarkSceneDirty(netObject.gameObject.scene);
        }
    }

    static EzNetObjectEditor()
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
        if (Application.isPlaying)
            return;

        EzNetObject[] networkViewResources = Resources.FindObjectsOfTypeAll<EzNetObject>();

        if (networkViewResources.All(view => IsPrefab(view.gameObject)))
            return;

        int targetViewId = networkViewResources.FirstOrDefault(view => !IsPrefab(view.gameObject))!.gameObject.scene.buildIndex * 1000 + 1;

        foreach (EzNetObject view in networkViewResources)
        {
            if (view.objectType == EzNetObjectType.Static)
            {
                if (IsPrefab(view.gameObject))
                {
                    // prefabs should use 0 as ViewID and sceneViewId
                    if (view.id != "")
                    {
                        view.id = "";
                        EditorUtility.SetDirty(view);
                    }
                    continue;   // skip prefabs in further processing
                }

                if (targetViewId > (networkViewResources.FirstOrDefault(view => !IsPrefab(view.gameObject))!.gameObject.scene.buildIndex * 1000) + 1000)
                {
                    Debug.LogError("허용된 NetworkView 개수를 넘었습니다.");
                    continue;
                }

                if (view.id != targetViewId.ToString())
                {
                    view.id = targetViewId.ToString();
                    EditorUtility.SetDirty(view);
                }

                targetViewId++;
            }
            else
            {
                view.id = "";
            }
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

    [Obsolete]
    private void _OnInspectorGUI()
    {
        /*return;
        base.OnInspectorGUI();

        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();

        EzNetObject myComponent = (EzNetObject)target;
        if (myComponent != null)
        {
            if (myComponent.objectType == EzNetObjectType.Static)
            {
                if (myComponent.id == "")
                {
                    if (GUILayout.Button("Generate UUID"))
                    {
                        myComponent.id = System.Guid.NewGuid().ToString();
                        EditorUtility.SetDirty(myComponent);
                    }
                }
            }
            else
            {
                myComponent.id = "";
                EditorUtility.SetDirty(myComponent);
            }
        }

        GUILayout.EndHorizontal();*/
    }
}

#endif