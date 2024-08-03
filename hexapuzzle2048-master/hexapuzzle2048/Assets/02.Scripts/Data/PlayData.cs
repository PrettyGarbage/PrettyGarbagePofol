using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayData{

    private int _adContinueWatchCount;
    private int _score;
    private int _coin;
    private int[,] _cells;
    private LastBlocks _lastParts;
    private ItemUseCountInfo _itemUseCountInfo;

    public PlayData(int score, int coin, int[,] cells, LastBlocks lastParts, ItemUseCountInfo itemUseCountInfo, int adContinueWatchCount){
        _score = score;
        _coin = coin;
        _cells = cells;
        _lastParts = lastParts;
        _itemUseCountInfo = itemUseCountInfo;
        _adContinueWatchCount = adContinueWatchCount;
    }

    public int Score
    {
        get
        {
            return _score;
        }
    }

    public int[,] Cells
    {
        get
        {
            return _cells;
        }
    }

    public LastBlocks LastParts
    {
        get
        {
            return _lastParts;
        }
    }

    public ItemUseCountInfo ItemUseCountInfo
    {
        get
        {
            return _itemUseCountInfo;
        }
    }

    public int AdContinueWatchCount
    {
        get
        {
            return _adContinueWatchCount;
        }
    }

    public int Coin
    {
        get
        {
            return _coin;
        }
    }

    public void Print(){
        Debug.Log("_score : " + _score + "/ AdContinueWatchCount : " + AdContinueWatchCount);
        _lastParts.Print();
        for (int i = 0; i < GameConstants.CELL_ARRAY_SIZE; i++)
        {
            for (int j = 0; j < GameConstants.CELL_ARRAY_SIZE; j++)
            {
                Debug.Log( i+"-"+j + " : " + _cells[i,j]);
            }    
        }
        for (int i = 0; i < ItemUseCountInfo.ItemUseCountArray.Length; i++)
        {
            Debug.Log("user item - " +  (ItemType)i + " : " + ItemUseCountInfo.ItemUseCountArray[i]);            
        }
    }
}