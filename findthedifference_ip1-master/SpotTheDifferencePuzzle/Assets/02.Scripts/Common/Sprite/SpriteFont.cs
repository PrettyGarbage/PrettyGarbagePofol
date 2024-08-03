using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct SpriteInfo
{
    public string text;
    public Sprite sprite;
}

public class SpriteFont : SpriteFontBase<Image>
{

    override protected void SetSprite(Image addImage, Sprite sprite, int index)
    {
        addImage.rectTransform.localPosition = new Vector3(GetFontPosX(index), 0, 0);
        addImage.rectTransform.localRotation = Quaternion.identity;
        addImage.rectTransform.localScale = Vector3.one;
        addImage.rectTransform.sizeDelta = Vector3.one * FontSize;
        addImage.sprite = sprite;
        addImage.color = Color;
    }

    override public void UpdateColor()
    {
        for (int i = 0; i < imageList.Count; i++)
        {
            imageList[i].color = Color;
        }
    }

    override public void removeList(int removeSize)
    {
        base.removeList(removeSize);

        for (int i = 0; i < imageList.Count; i++)
        {
            imageList[i].rectTransform.localPosition = new Vector3(GetFontPosX(i), 0, 0);
        }
    }

    public override void Dispose()
    {

    }


}