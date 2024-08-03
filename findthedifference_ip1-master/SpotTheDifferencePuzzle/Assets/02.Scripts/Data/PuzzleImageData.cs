using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using UnityEngine.UI;

[Serializable]
public class PuzzleImagePart
{
    [SerializeField]
    private Sprite _partSprite;
    [SerializeField]
    private Vector2 _position;
    [SerializeField]
    private ImageDifficulty _imageDifficulty = ImageDifficulty.EASY;

    public ImageDifficulty ImageDifficulty
    {
        get
        {
            return _imageDifficulty;
        }

        set
        {
            _imageDifficulty = value;
        }
    }

    public Sprite PartSprite
    {
        get
        {
            return _partSprite;
        }

        set
        {
            _partSprite = value;
        }
    }

    public Vector2 Position
    {
        get
        {
            return _position;
        }

        set
        {
            _position = value;
        }
    }

}

#if UNITY_EDITOR
[CreateAssetMenu(fileName = "PuzzleImageData.asset", menuName = "gbros/PuzzleImageData", order = 1)]
#endif
public class PuzzleImageData : ScriptableObject
{
    [SerializeField]
    private string _id;
    [SerializeField]
    private Sprite _imageSprite;
    
    
    [Header("PuzzleImagePart")]
    [SerializeField]
    private PuzzleImagePart[] _puzzleImageParts;

    public int PuzzlePartCount
    {
        get
        {
            return PuzzleImageParts == null ? 0 : PuzzleImageParts.Length;
        }
    }

    public PuzzleImagePart[] PuzzleImageParts
    {
        get
        {
            return _puzzleImageParts;
        }
    }

    public string Id
    {
        get
        {
            return _id;
        }
    }

    public Sprite ImageSprite
    {
        get
        {
            return _imageSprite;
        }

        set {
            _imageSprite = value;
        }
    }

    // public bool IsCorrectPoint(Vector2 localPoint, float range)
    // {
    //     bool isCorrect = false;
    //     for (int i = 0; i < puzzleImageParts.Length; i++)
    //     {
    //         if (!puzzleImageParts[i].isEnd)
    //         {
    //             float sqrMagnitude = Vector2.SqrMagnitude(puzzleImageParts[i].position - localPoint);
    //             isCorrect = (sqrMagnitude* sqrMagnitude) <= range;
    //             if (isCorrect)
    //                 puzzleImageParts[i].isEnd = true;
    //         }
    //     }

    //     return isCorrect;
    // }
#if UNITY_EDITOR
    public static PuzzleImageData CreateAssest(string name, Sprite imageSprite, PuzzleImagePart[] puzzleImageParts)
    {
        PuzzleImageData puzzleImageData = null;
        string path = "Assets/07.Data/03.PuzzleData/" + name + ".asset";
        puzzleImageData = (PuzzleImageData)AssetDatabase.LoadAssetAtPath(path, typeof(PuzzleImageData));
        Debug.Log("PuzzleImageData path : "+ path+ " | IsValidFolder:  "+ puzzleImageData!=null);

        if(puzzleImageData!=null){
            
            for (int i = 0; i < puzzleImageData.PuzzleImageParts.Length; i++)
            {
                puzzleImageData.PuzzleImageParts[i].Position = puzzleImageParts[i].Position;
            }

            return puzzleImageData;
        }
        else {
            puzzleImageData = CreateInstance<PuzzleImageData>();
            puzzleImageData.name = name;
            puzzleImageData._id = name;
            puzzleImageData._imageSprite = imageSprite;
            puzzleImageData._puzzleImageParts = puzzleImageParts;
            AssetDatabase.CreateAsset(puzzleImageData, "Assets/07.Data/03.PuzzleData/" + name + ".asset");
        }

        AssetDatabase.Refresh();
        return puzzleImageData;
    }
#endif



}


