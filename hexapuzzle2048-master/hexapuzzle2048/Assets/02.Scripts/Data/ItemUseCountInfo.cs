using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class  ItemUseCountInfo
{
    private int[] _itemUseCountArray;
    public ItemUseCountInfo(){
        _itemUseCountArray = new int[Enum.GetNames(typeof(ItemType)).Length];
    }

    public int[] ItemUseCountArray
    {
        get
        {
            return _itemUseCountArray;
        }
    }

    public ItemUseCountInfo AddCount(ItemType type, int count){
        _itemUseCountArray[(int)type] = count;
        return this;
    }

    public int GetCount(ItemType type){
        return _itemUseCountArray[(int)type];
    }

    public static ItemUseCountInfo GetInstance(int[] itemUsed){
        ItemUseCountInfo itemUsedInfo = new ItemUseCountInfo();
        for (int i = 0, max = itemUsed.Length; i < max; ++i)
		{
			itemUsedInfo.AddCount((ItemType)i, itemUsed[i]);
		}

        return itemUsedInfo;
    }
}
