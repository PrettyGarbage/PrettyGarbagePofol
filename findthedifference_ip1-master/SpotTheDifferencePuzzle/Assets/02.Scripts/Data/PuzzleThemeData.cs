using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PuzzleThemeData.asset", menuName = "gbros/PuzzleThemeData", order = 1)]
public class PuzzleThemeData : ScriptableObject{

    [SerializeField]
	private string _themeId;

    [SerializeField]
    private Sprite _themeImage;

    [SerializeField]
    private PuzzleImageData[] _puzzleImageDatas;

    public string ThemeId
    {
        get
        {
            return _themeId;
        }
    }

    public PuzzleImageData[] PuzzleImageDatas
    {
        get
        {
            return _puzzleImageDatas;
        }
    }

    public Sprite ThemeImage
    {
        get
        {
            return _themeImage;
        }

        set
        {
            _themeImage = value;
        }
    }
}
