using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class LocalizedTextWindow : EditorWindow
{
    public LocalizationData localizationData;

    public string createKey;
    public string createValueKr;
    public string createValueEn;
    public string createValueJp;

    public string searchString = string.Empty;
    Vector2 scrollPosition = Vector2.zero;

    [MenuItem("Window/gbros/Localized Text Editor")]
    static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(LocalizedTextWindow)).Show();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Load data"))
        {
            LoadGameData();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Separator();

        if (localizationData == null)
        {
            if (GUILayout.Button("Create new data"))
            {
                CreateNewData();
            }
        }
        else
        {
            GUILayout.Label("Create Data", EditorStyles.boldLabel);
            createKey = EditorGUILayout.TextField("Key", createKey);
            EditorGUILayout.LabelField("한국어");
            createValueKr = EditorGUILayout.TextArea(createValueKr);
            EditorGUILayout.LabelField("English");
            createValueEn = EditorGUILayout.TextArea(createValueEn);
            EditorGUILayout.LabelField("日本語");
            createValueJp = EditorGUILayout.TextArea(createValueJp);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset Data"))
            {
                ResetCreateInfo();
            }

            if (GUILayout.Button("Create Data"))
            {
                if (createKey.Length == 0)
                {
                    EditorUtility.DisplayDialog("Create Data", "KeyName is Empty", "ok");
                    return;
                }

                if (localizationData.IsExistKey(createKey))
                {
                    EditorUtility.DisplayDialog("Create Data", "Key Duplicated " + createKey, "ok");
                    return;
                }

                if (localizationData.AddData(createKey, createValueKr, createValueEn, createValueJp))
                {
                    EditorUtility.DisplayDialog("Create Data", "Success", "ok");
                    ResetCreateInfo();
                }
                else
                {
                    EditorUtility.DisplayDialog("Create Data", "Fail", "ok");
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.Separator();

            if (GUILayout.Button("Save data"))
            {
                SaveGameData();
            }

            GUILayout.Label("Data List", EditorStyles.boldLabel);

            GUI.color = Color.green;
            searchString = EditorGUILayout.TextField("Search Key", searchString).ToUpper();
            GUI.color = Color.white;

            EditorGUILayout.Space();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, true, true);
            for (int i = 0; i < localizationData.textItems.Count; i++)
            {
                LocalizationItem localizationItem = localizationData.textItems[i];
                if (searchString.Length == 0 || localizationItem.key.IndexOf(searchString) >= 0)
                {

                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginDisabledGroup(true);

                    GUI.color = Color.magenta;
                    EditorGUILayout.TextField("Key Name", localizationItem.key);
                    EditorGUI.EndDisabledGroup();

                    GUI.color = Color.red;
                    if (GUILayout.Button("Delete"))
                    {
                        localizationData.RemoveData(localizationItem.key);
                    }
                    GUI.color = Color.white;
                    EditorGUILayout.EndHorizontal();

                    GUI.color = Color.white;

                    EditorGUILayout.LabelField("한국어");
                    localizationItem.valueKr = EditorGUILayout.TextArea(localizationItem.valueKr);
                    EditorGUILayout.LabelField("English");
                    localizationItem.valueEn = EditorGUILayout.TextArea(localizationItem.valueEn);
                    EditorGUILayout.LabelField("日本語");
                    localizationItem.valueJp = EditorGUILayout.TextArea(localizationItem.valueJp);
                    
                    EditorGUILayout.Separator();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                }
            }
            GUILayout.EndScrollView();

        }
       

    }

    private void ResetCreateInfo()
    {
        createKey = string.Empty;
        createValueKr = string.Empty;
        createValueEn = string.Empty;
        createValueJp = string.Empty;        
    }

    private void LoadGameData()
    {
        string filePath = EditorUtility.OpenFilePanel("Select localization data file", Application.streamingAssetsPath, "json");

        if (!string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);

            localizationData = JsonUtility.FromJson<LocalizationData>(dataAsJson);
        }
    }

    private void SaveGameData()
    {
        string filePath = EditorUtility.SaveFilePanel("Save localization data file", Application.streamingAssetsPath, "", "json");

        if (!string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = JsonUtility.ToJson(localizationData);
            File.WriteAllText(filePath, dataAsJson);

            dataAsJson = JsonUtility.ToJson(localizationData.GetLocalizationItemList(LangType.KR));
            Debug.Log(dataAsJson);
            File.WriteAllText(LocalizationManager.GetDataFilePath(LangType.KR), dataAsJson);

            dataAsJson = JsonUtility.ToJson(localizationData.GetLocalizationItemList(LangType.JP));
            File.WriteAllText(LocalizationManager.GetDataFilePath(LangType.JP), dataAsJson);

            dataAsJson = JsonUtility.ToJson(localizationData.GetLocalizationItemList(LangType.EN));
            File.WriteAllText(LocalizationManager.GetDataFilePath(LangType.EN), dataAsJson);

            //Enum
            File.WriteAllText("Assets/02.Scripts/Common/Localization/LocalizationTextKey.cs" , localizationData.GetEnumStringFromKeys());

            AssetDatabase.Refresh();
        }
    }

    private void CreateNewData()
    {
        localizationData = new LocalizationData();
    }

}