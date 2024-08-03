using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteFontBase<T> : BaseObject where T : Component
{
    public enum SpriteFontAlignType
    {
        LEFT, CENTER, RIGHT
    }

    [SerializeField]
    protected string _text = string.Empty;
    [SerializeField]
    private SpriteFontAlignType _spriteFontAlignType= SpriteFontAlignType.CENTER;
    [SerializeField]
    private float _wordSpace = 1;
    [SerializeField]
    private float _fontSize = 3;
    public List<T> imageList;

    [SerializeField]
    protected SpriteInfo[] spriteInfoArray;
    public Color color;

    protected int _lastTextSize;
    protected string _lastText = string.Empty;

    public SpriteInfo[] SpriteInfoArray
    {
        get
        {
            return spriteInfoArray;
        }

        set
        {
            spriteInfoArray = value;
        }
    }

    public string Text
    {
        get
        {
            return _text;
        }

        set
        {
            _text = value;
            if (!_lastText.Equals(_text))
            {
                _lastText = _text;
                UpdateText();
            }
        }
    }

    public Color Color
    {
        get
        {
            return color;
        }

        set
        {
            color = value;
            UpdateColor();
        }
    }


    public float WordSpace
    {
        get
        {
            return _wordSpace;
        }

        set
        {
            _wordSpace = value;
            UpdateText();
        }
    }

    public float FontSize
    {
        get
        {
            return _fontSize;
        }

        set
        {
            _fontSize = value;
            UpdateText();
        }
    }

    public SpriteFontAlignType AlignType
    {
        get
        {
            return _spriteFontAlignType;
        }

        set
        {
            _spriteFontAlignType = value;
            UpdateText();
        }
    }

    virtual public void UpdateColor()
    {
        
    }
    public void UpdateText()
    {
        char[] strArray = Text.ToCharArray();

        for (int i = 0; i < strArray.Length; i++)
        {
            bool isFind = false;
            for (int j = 0; j < SpriteInfoArray.Length; j++)
            {
                if (SpriteInfoArray[j].text!=null && SpriteInfoArray[j].text.Equals(strArray[i].ToString()))
                {
                    isFind = true;
                    AddImageFont(i, SpriteInfoArray[j].sprite);
                }                
            }

            if(!isFind)
                AddImageFont(i, null);

        }

        removeList(_lastTextSize - strArray.Length);
        _lastTextSize = strArray.Length;
    }

    public void AddImageFont(int index, Sprite sprite)
    {
        if (imageList == null)
        {
            imageList = new List<T>();
        }

        if (imageList.Count > index)
        {
            SetSprite(imageList[index], sprite, index);
        }
        else
        {
            T addImage = new GameObject("image" + index).AddComponent<T>();
            addImage.gameObject.layer = transform.parent.gameObject.layer;
            addImage.transform.SetParent(transform);
            SetSprite(addImage, sprite, index);
            imageList.Add(addImage);
        }
    }

    virtual protected void SetSprite(T addImage, Sprite sprite, int index)
    {

    }


    protected float GetFontPosX(int index)
    {
        int size = imageList.Count;
        if (size == 1) return 0f;

        switch (AlignType)
        {
            case SpriteFontAlignType.LEFT:
                return index * WordSpace;
            case SpriteFontAlignType.CENTER:
                float startPosX = (size * WordSpace) / 2 * -1f;
                return startPosX + (WordSpace * (index) + WordSpace / 2);
            case SpriteFontAlignType.RIGHT:
                return (size-index) * -WordSpace;            
        }
        return 0f;
    }

    virtual public void removeList(int removeSize)
    {
        if (imageList == null) return;

        for (int i = 0; i < removeSize; i++)
        {
            DestroyImmediate(imageList[imageList.Count - 1].gameObject);
            imageList.RemoveAt(imageList.Count - 1);

        }

        for (int i = 0; i < imageList.Count; i++)
        {
            //imageList[i].rectTransform.localPosition = new Vector3(GetFontPosX(i), 0, 0);
        }
    }

    public override void Dispose()
    {

    }


}
