using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteFontRenderer : SpriteFontBase<SpriteRenderer>
{

    override protected void SetSprite(SpriteRenderer addImage, Sprite sprite, int index)
    {
        addImage.transform.localPosition = new Vector3(GetFontPosX(index), 0, 0);
        addImage.transform.localRotation = Quaternion.identity;
        addImage.transform.localScale = Vector3.one * FontSize;
        addImage.sprite = sprite;
        addImage.color = Color;
    }

    override public void removeList(int removeSize)
    {
        base.removeList(removeSize);

        for (int i = 0; i < imageList.Count; i++)
        {
            imageList[i].transform.localPosition = new Vector3(GetFontPosX(i), 0, 0);
        }
    }

    public override void Dispose()
    {

    }


}