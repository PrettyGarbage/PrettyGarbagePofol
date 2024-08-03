using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class AudioDataEditorWindow : EditorWindow
{

    [SerializeField]
	public AudioDataBase audioDataBase;
    Vector2 scrollPosition = Vector2.zero;
    //Create
    string createKeyName;
    bool createIsLoop = false;
    float createVolumeRate = 1f;
    AudioClip createClip;
    AudioType createAudioType = AudioType.FX;
    
    //Search
    string searchString = string.Empty;

    [MenuItem("Window/gbros/AudioDataBase")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(AudioDataEditorWindow));
    }

    void OnGUI()
    {
        
        GUILayout.Label ("DataBase File", EditorStyles.boldLabel);
		audioDataBase = (AudioDataBase)EditorGUILayout.ObjectField("DataBase", audioDataBase, typeof(AudioDataBase));
        if(audioDataBase==null){
            if(GUILayout.Button("Create DataBase File"))
            {
                audioDataBase = AudioDataBase.CreateAssest();
            }
            return;
        }

        GUILayout.Label ("Create Data", EditorStyles.boldLabel);
        createKeyName = EditorGUILayout.TextField ("Key Name", createKeyName); 
        createVolumeRate = EditorGUILayout.FloatField ("Volume Rate", createVolumeRate);
        createClip = (AudioClip)EditorGUILayout.ObjectField ("Clip", createClip, typeof(AudioClip));
        createAudioType = (AudioType)EditorGUILayout.EnumPopup("Audio Type", createAudioType);
        createIsLoop = EditorGUILayout.Toggle("Is Loop", createIsLoop);

        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Reset Data"))
        {
            ResetCreateInfo();
        }

        if(GUILayout.Button("Create Data"))
        {
            if(createKeyName.Length == 0){
                EditorUtility.DisplayDialog("Create Data", "KeyName is Empty", "ok");
                return;
            }
            if(createClip == null){
                EditorUtility.DisplayDialog("Create Data", "Clip is Empty", "ok");
                return;
            }

            if(audioDataBase.AddData(createKeyName, new AudioClipInfo(createClip, createVolumeRate, createAudioType, createIsLoop))){
                EditorUtility.DisplayDialog("Create Data", "Success", "ok");
                EditorUtility.SetDirty(audioDataBase);
                AssetDatabase.SaveAssets();
                ResetCreateInfo();
            }else {
                EditorUtility.DisplayDialog("Create Data", "Fail", "ok");
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Separator();
       

        GUILayout.Label ("DataBase List", EditorStyles.boldLabel);
        
        GUI.color = Color.green;
        searchString = EditorGUILayout.TextField ("Search Key", searchString); 
        GUI.color = Color.white;

        EditorGUILayout.Space();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, true, true); 
        for (int i = 0; i < audioDataBase.AudioDataInfoList.Count; i++)
        {
            AudioDataInfo audioDataInfo = audioDataBase.AudioDataInfoList[i];
            if(searchString.Length == 0 || audioDataInfo.Key.IndexOf(searchString) >= 0){
                EditorGUI.BeginDisabledGroup(true);
                AudioClipInfo audioClipInfo = audioDataInfo.AudioClipInfo;
                GUI.color = Color.magenta;
                EditorGUILayout.TextField ("Key Name", audioDataInfo.Key); 
                GUI.color = Color.white;
                EditorGUILayout.FloatField ("Volume Rate", audioClipInfo.VolumeRate);
                EditorGUILayout.ObjectField ("Clip", audioClipInfo.Clip, typeof(AudioClip));
                EditorGUILayout.EnumPopup("Audio Type", audioClipInfo.Type);
                EditorGUILayout.Toggle("Is Loop", audioClipInfo.IsLoop);
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.BeginHorizontal();
                if(GUILayout.Button("Play"))
                {
                    //_audioDataBase.RemoveData(audioDataInfo.Key);
                    if(!IsClipPlaying(audioClipInfo.Clip))
                        PlayClip(audioClipInfo.Clip);
                }
                if(GUILayout.Button("Stop"))
                {
                    //_audioDataBase.RemoveData(audioDataInfo.Key);
                    StopClip(audioClipInfo.Clip);
                }
                GUI.color = Color.red;
                if(GUILayout.Button("Delete"))
                {
                    audioDataBase.RemoveData(audioDataInfo.Key);
                }
                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                
            }
        } 
        GUILayout.EndScrollView();

    }

    private void ResetCreateInfo(){
        createKeyName = string.Empty;
        createIsLoop = false;
        createVolumeRate = 1f;
        createClip = null;
        createAudioType = AudioType.FX;
    }

    public static void PlayClip(AudioClip clip) {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "PlayClip",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new System.Type[] {
                typeof(AudioClip)
            },
            null
        );
        method.Invoke(
            null,
            new object[] {
                clip
            }
        );
    }

    public static void StopClip(AudioClip clip) {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass =
              unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "StopClip",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new System.Type[] {
                typeof(AudioClip)
            },
            null
        );
        method.Invoke(
            null,
            new object[] {clip}
        );
    }

    public static bool IsClipPlaying(AudioClip clip) {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass =
              unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "IsClipPlaying",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new System.Type[] {
                typeof(AudioClip)
            },
            null
        );
        return (bool)method.Invoke(
            null,
            new object[] {clip}
        );
    }



}