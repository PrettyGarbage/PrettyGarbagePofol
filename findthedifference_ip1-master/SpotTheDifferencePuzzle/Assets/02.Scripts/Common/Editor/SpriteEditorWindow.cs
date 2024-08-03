using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class  EtextureInfo
{
    public Texture texture;
    public string tagName;
    public List<GameObject> goList = new List<GameObject>();
    public bool isShowGoList;
    public EtextureInfo(Texture texture, string tagName){
        this.texture = texture;
        this.tagName = tagName;
    }

    public void AddGameobject(GameObject go){
        goList.Add(go);
    }
}

public class SpriteEditorWindow : EditorWindow
{
	private List<Scene> _sceneList = new List<Scene>();
    
    private int sceneCount;

    Vector2 scrollPosition = Vector2.zero;

    private List<EtextureInfo> _etextureInfoList = new List<EtextureInfo>();
    

    [MenuItem("Window/gbros/SpriteEditor")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(SpriteEditorWindow));
        
    }
    
    void OnGUI()
    {
        if(sceneCount != SceneManager.sceneCount){
            SetSceneList();
        }
        
        GUILayout.Label ("Modify Texture PackingTag", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if(GUILayout.Button("Refresh"))
        {
            SetSceneList();
        }
    
        GUILayout.Label ("Scene List", EditorStyles.boldLabel);
        for (int i = 0; i < _sceneList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField (_sceneList[i].name); 
            if(GUILayout.Button("Show Sprites"))
            {
                SetTextureList(_sceneList[i]);
            }
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Label ("Texture List", EditorStyles.boldLabel);
        if(GUILayout.Button("All Apply"))
        {
            ApplyAllSpritePackingTag();
        }
        EditorGUILayout.BeginHorizontal();
            GUILayout.Label ("texture", EditorStyles.boldLabel);
            GUILayout.Label ("gameobject", EditorStyles.boldLabel);
            GUILayout.Label ("PackingTag", EditorStyles.boldLabel);
            GUILayout.Label ("", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, true, true); 
        for (int i = 0; i < _etextureInfoList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.ObjectField (_etextureInfoList[i].texture, typeof(Texture));
            
            if(_etextureInfoList[i].isShowGoList){
                EditorGUILayout.BeginVertical();
                List<GameObject> goList = _etextureInfoList[i].goList;
                for(int j = 0; j < goList.Count; j++)
                {
                    goList[j] = (GameObject)EditorGUILayout.ObjectField(goList[j], typeof(GameObject));
                }

                EditorGUILayout.EndVertical();
            }
            

            _etextureInfoList[i].tagName = EditorGUILayout.TextField (_etextureInfoList[i].tagName); 
            if(GUILayout.Button("Apply"))
            {
                ApplySpritePackingTag(_etextureInfoList[i].texture, _etextureInfoList[i].tagName);
            }
            if(GUILayout.Button("GameObjects"))
            {
                _etextureInfoList[i].isShowGoList = !_etextureInfoList[i].isShowGoList;
            }
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
		

    }

    private void ApplyAllSpritePackingTag()
    {
        for (int i = 0; i < _etextureInfoList.Count; i++)
        {
            ApplySpritePackingTag(_etextureInfoList[i].texture, _etextureInfoList[i].tagName);
        }
    }

    private void SetTextureList(Scene scene){
        GameObject[] gos = scene.GetRootGameObjects();
        _etextureInfoList.Clear();
        
        
        for (int i = 0; i < gos.Length; i++)
        {
            Image[] images = gos[i].GetComponentsInChildren<Image>(true);
            for (int j = 0; j < images.Length; j++)
            {
                AddTextureList(images[j].gameObject, images[j].mainTexture);
            }

            SpriteRenderer[] spriteRenderers = gos[i].GetComponentsInChildren<SpriteRenderer>(true);
            for (int j = 0; j < spriteRenderers.Length; j++)
            {
                AddTextureList(spriteRenderers[j].gameObject, spriteRenderers[j].sprite.texture);
            }
        }

        _etextureInfoList.Sort((a,b)=>{
            return a.texture.name.CompareTo(b.texture.name);
        });
        
    }

    private void AddTextureList(GameObject go, Texture texture){

        for (int i = 0; i < _etextureInfoList.Count; i++)
        {
            if(_etextureInfoList[i].texture.Equals(texture)){
                _etextureInfoList[i].goList.Add(go);
                return;
            }
        }
        EtextureInfo etextureInfo = new EtextureInfo(texture, GetSpritePackingTag(texture));
        etextureInfo.AddGameobject(go);
        _etextureInfoList.Add(etextureInfo);
    }

    private  void SetSceneList(){
        sceneCount = SceneManager.sceneCount;
        _sceneList.Clear();
        _etextureInfoList.Clear();        
        for (int i = 0; i < sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            GameObject[] go = scene.GetRootGameObjects();
            Debug.Log("objs : " +i +" / " + scene.name + " / " + go[0].name);
            _sceneList.Add(scene);
        }
    }

    private void ApplySpritePackingTag(Texture texture, string spritePackingTag){
        
        string assetPath = AssetDatabase.GetAssetPath( texture );
        var tImporter = AssetImporter.GetAtPath( assetPath ) as TextureImporter;
        if ( tImporter != null )
        {
            //tImporter.textureCompression = textureImporterCompression;
            tImporter.spritePackingTag = spritePackingTag;
            AssetDatabase.ImportAsset( assetPath );
            AssetDatabase.Refresh();
        }
    }

    private string GetSpritePackingTag(Texture texture){
        
        string assetPath = AssetDatabase.GetAssetPath( texture );
        var tImporter = AssetImporter.GetAtPath( assetPath ) as TextureImporter;
        if ( tImporter != null )
        {
            //tImporter.textureCompression = textureImporterCompression;
            return tImporter.spritePackingTag;
        }
        return string.Empty;
    }


}