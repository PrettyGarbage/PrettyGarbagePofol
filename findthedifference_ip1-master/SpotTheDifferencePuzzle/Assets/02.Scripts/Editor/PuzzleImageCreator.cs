using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

public class PuzzleImageCreator
{

    static string directoryPath;

    // [MenuItem("Assets/Resize")]
    // static void Resize()
    // {
    //     for (int i = 0; i < Selection.objects.Length; i++)
    //     {
    //         string assetPath = AssetDatabase.GetAssetPath(Selection.objects[i]);          
    //         Debug.Log("path : " + assetPath + " / " + Path.GetDirectoryName(assetPath)); 

    //         Object obj = Selection.objects[i];

    //         EditorUtility.DisplayProgressBar("Resize", obj.name, (float)(i+1)/(float)Selection.objects.Length);

    //         if(obj is Texture){
    //             SetTextureCustom((Texture)obj, TextureImporterCompression.Compressed, TextureImporterFormat.ETC2_RGBA8, false);                
    //         }
    //         else {
    //             Debug.LogError("Resize Error : " + obj.name + " is not Texture!" );
    //         }
            
    //     }
    //     EditorUtility.ClearProgressBar();
    // }

    [MenuItem("Assets/gbros/CreatePuzzleImageData")]
    static void CreatePuzzleImageData()
    {
        Debug.Log("Selection.objects : " + Selection.objects.Length);
        for (int i = 0; i < Selection.objects.Length; i++)
        {
            string assetPath = AssetDatabase.GetAssetPath(Selection.objects[i]);          
            Debug.Log("path : " + assetPath + " / " + Path.GetDirectoryName(assetPath)); 
            
            directoryPath = Path.GetDirectoryName(assetPath);

            Object obj = Selection.objects[i];
            
            if(obj is Texture2D){
                CreateTexture2D((Texture2D)obj);
                EditorUtility.DisplayProgressBar("CreatePuzzleImageData", obj.name, (float)(i+1)/(float)Selection.objects.Length);
            }
            else {
                Debug.LogError("CreatePuzzleImageData Error : " + Selection.objects[i].name + " is not Texture!" );
            }
            EditorUtility.ClearProgressBar();
            Thread.Sleep(500);
            
        }
        AssetDatabase.Refresh();
    }

    static void CreateTexture2D(Texture2D texture)
    {
        Debug.Log(texture.format);
        Debug.Log(texture.width + " / " + texture.height);
        if(texture.name.IndexOf("PUZ") < 0){
            return;
        }


        bool[,] ignorePixelPoint = new bool[texture.width, texture.height];
        List<PuzzleImagePart> puzzleImagePartList = new List<PuzzleImagePart>();

        SetTextureCustom(texture, TextureImporterCompression.Compressed, TextureImporterFormat.ETC2_RGBA8, true);
        //SetTextureReadable(texture);

        int index = 0;
        Vector2 lastLeftPoint = Vector2.zero;
        for (int i = 0; i < 15; i++)
        {
            //Search Image Pixel Point
            lastLeftPoint = SearchImagePixel(texture, ignorePixelPoint);
            if (lastLeftPoint != Vector2.zero)
            {
                ignorePixelPoint = CreateImageFileAndData(index, lastLeftPoint, texture, ignorePixelPoint, puzzleImagePartList);
                index = puzzleImagePartList.Count;
            }
            else
            {
                Sprite originSprite = (Sprite)AssetDatabase.LoadAssetAtPath(directoryPath+"/"+texture.name.Replace("PUZ","ORI")+".jpg", typeof(Sprite));
                PuzzleImageData puzzleImageData = PuzzleImageData.CreateAssest(texture.name, originSprite, puzzleImagePartList.ToArray());

                for (int h = 0; h < puzzleImageData.PuzzleImageParts.Length; h++)
                {
                    Sprite sprite = (Sprite)AssetDatabase.LoadAssetAtPath(GetResourceFilePngPath(texture.name, h), typeof(Sprite));
                    SetSpritePackingTag(sprite.texture, texture.name);

                    puzzleImageData.PuzzleImageParts[h].PartSprite = sprite;
                }
                EditorUtility.SetDirty(puzzleImageData);
                Debug.Log("Finish");


                break;
            }
        }
    }

    static bool[,] CreateImageFileAndData(int index, Vector2 leftPoint, Texture2D texture, bool[,] ignorePixelPoint, List<PuzzleImagePart> puzzleImagePartList)
    {

        Debug.Log(leftPoint);
        PuzzleImagePart puzzleImagePart;
        Rect rect = SearchImageRect(leftPoint, texture);

        int startX = (int)(rect.x<=0 ? 0: rect.x);
        int startY = (int)(rect.y<=0 ? 0: rect.y);

        Debug.Log(index);
        Debug.Log(rect);

        if(rect.width < 20){
            for (int i = 0; i < rect.width; i++)
            {
                for (int j = 0; j < rect.width; j++)
                {
                    if(texture.width > i + startX && texture.height > i + startY)
                        ignorePixelPoint[i + startX, j + startY] = true;
                }
            }

            return ignorePixelPoint;
        }

        Texture2D partsTexture = new Texture2D((int)rect.width, (int)rect.width, texture.format, true);

        for (int i = 0; i < rect.width; i++)
        {
            for (int j = 0; j < rect.width; j++)
            {
                if(texture.width > i + startX && texture.height > j + startY){
                    try
                    {
                        ignorePixelPoint[i + startX, j + startY] = true;
                        partsTexture.SetPixel(i, j, texture.GetPixel(i + startX, j + startY));    
                    }
                    catch (System.Exception)
                    {
                        Debug.Log("Exception i : " + (i + startX) + " / texture.width : " + texture.width + "/ i + startY :" + (i + startY)+ "/ texture.height : " + texture.height);
                        throw;
                    }
                    
                }
                if(index == 5 && (i + startX) == 466){
                    //Debug.Log("i : " + (i + startX) + "/ j : " + (j + startY)+ "/ " + ignorePixelPoint[i + startX, j + startY]);
                }
            }
        }
        
        partsTexture.Apply();
        UploadPNG(texture.name, partsTexture, index);

        puzzleImagePart = new PuzzleImagePart();
        puzzleImagePart.Position = new Vector2(rect.x + rect.width / 2, rect.y + rect.width / 2);
        puzzleImagePart.ImageDifficulty = rect.width > 100 ? ImageDifficulty.EASY : rect.width > 50 ? ImageDifficulty.NORMAL : ImageDifficulty.HARD;
        puzzleImagePartList.Add(puzzleImagePart);

        return ignorePixelPoint;
    }

    static Vector2 SearchImagePixel(Texture2D texture, bool[,] ignorePixelPoint)
    {
        Vector2 retVector2 = Vector2.zero;
        for (int i = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.height; j++)
            {
                if (!ignorePixelPoint[i, j] && texture.GetPixel(i, j).a > 0.2)
                {
                    Debug.Log("SearchImagePixel : i " + i + " / j : " + j + " / color : " + texture.GetPixel(i, j));
                    retVector2 = new Vector2(i, j);
                    Vector2 vector2 = retVector2;
                    while (texture.GetPixel((int)vector2.x, (int)vector2.y).a != 0 && texture.height > vector2.y)
                    {
                        vector2.y += 1;
                    }
                    retVector2.y = retVector2.y + (vector2.y - retVector2.y) / 2;
                    return retVector2;
                }
            }
        }

        return Vector2.zero;
    }

    static Rect SearchImageRect(Vector2 leftPoint, Texture2D texture)
    {
        Vector2 point = leftPoint;
        while (texture.GetPixel((int)point.x, (int)point.y).a != 0 && texture.width > point.x)
        {
            point.x += 1;
        }

        float size = point.x - leftPoint.x;

        Rect rect = new Rect(leftPoint.x , leftPoint.y - size / 2, size, size);
        Debug.Log(rect);
        rect.x -= 2f;
        rect.y -= 2f;
        rect.width += 4f;
        rect.height += 4f;

        return rect;
    }

    static void UploadPNG(string name, Texture2D tex, int index)
    {
        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();
        //Object.Destroy(tex);

        // For testing purposes, also write to a file in the project folder
        //string directory = Application.dataPath + "/Resources/" + GetPath(name);
        string directory = GetPath(name);
        string path = directory + GetFileName(name, index);

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (File.Exists(path))
        {
            File.Delete(path);
        }


        File.WriteAllBytes(path, bytes);

        //        PuzzleImageData.CreateAssest(name, )

    }

    static string GetFileName(string name, int index)
    {
        return name + "_" + index + ".png";
    }
    static string GetPath(string name)
    {
        return "Assets/04.GameResources/PuzzleImage/" + name + "/";
    }

    static string GetResourceFilePngPath(string name, int index)
    {
        return GetPath(name) + name + "_" + index + ".png";
    }

    static void SetTextureReadable(Texture texture){
        
        string assetPath = AssetDatabase.GetAssetPath( texture );
        var tImporter = AssetImporter.GetAtPath( assetPath ) as TextureImporter;
        if ( tImporter != null )
        {
            tImporter.isReadable = true;
            AssetDatabase.ImportAsset( assetPath );
            AssetDatabase.Refresh();
        }
    }

    static void SetSpritePackingTag(Texture texture, string spritePackingTag){
        
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

    static void SetTextureCustom(Texture texture, TextureImporterCompression textureImporterCompression, TextureImporterFormat textureImporterFormat, bool isReadable){
        
        string assetPath = AssetDatabase.GetAssetPath( texture );
        var tImporter = AssetImporter.GetAtPath( assetPath ) as TextureImporter;
        if ( tImporter != null)
        {
            tImporter.isReadable = isReadable;
            TextureImporterPlatformSettings platformSetting = new TextureImporterPlatformSettings();
            platformSetting.name = "Android";
            platformSetting.format = textureImporterFormat;
            platformSetting.textureCompression = textureImporterCompression;
            platformSetting.overridden = true;
            //platformSetting.maxTextureSize = GetHalfMaxTextureSize(texture);
            tImporter.SetPlatformTextureSettings(platformSetting);
            tImporter.name = tImporter.name.Trim();
            
            Debug.Log("assetPath : " +assetPath);

            AssetDatabase.ImportAsset( assetPath );
            AssetDatabase.Refresh();
        }
    }

    // static int GetHalfMaxTextureSize(Texture texture){

    //     if(texture.width >= 1024){
    //         return 1024;
    //     }else if(texture.width >= 512){
    //         return 512;
    //     }else if(texture.width >= 256){
    //         return 256;
    //     }
    //     else if(texture.width >= 128){
    //         return 128;
    //     }
    //     else if(texture.width >= 64){
    //         return 64;
    //     }
    //     else {
    //         return 64;
    //     }
    // }


}
#endif