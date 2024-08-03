using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

[Serializable]
public class CellSprite
{
    public Sprite sprite;
    public Color color = Color.white;
    public bool displayNumber = true;
}

#if UNITY_EDITOR
[CreateAssetMenu(fileName = "BoardTheme.asset", menuName = "gbros/Theme/Board Theme", order = 1)]
#endif
[Serializable]
public class BoardTheme : ScriptableObject
{
#if UNITY_EDITOR
    public const string PATH = "Assets/07.Data/Themes/BoardTheme.asset";

    public static BoardTheme CreateAsset()
    {
        BoardTheme theme = CreateInstance<BoardTheme>();
        AssetDatabase.CreateAsset(theme, PATH);
        AssetDatabase.Refresh();

        return theme;
    }
#endif

    [SerializeField] Sprite _tileSprite;
    [SerializeField] Sprite _explosionSprite;
    [SerializeField] CellSprite[] _cellSprites;
    

	public CellSprite GetCellSprite(int number)
	{
		--number;

		return (number >= 0 && number < _cellSprites.Length) ? _cellSprites[number] : null;
	}

	public Sprite tileSprite
	{
		get
		{
			return _tileSprite;
		}
	}

    public Sprite explosionSprite
    {
        get
        {
            return _explosionSprite;
        }
    }

    public CellSprite[] cellSprites
    {
        get
        {
            return _cellSprites;
        }
    }
}