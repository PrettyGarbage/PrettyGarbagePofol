using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class  LastBlocks
{
    private int[] _cells;
    private int _angleValue;

    public LastBlocks(int[] cells, int angleValue){
        _cells = cells;
        _angleValue = angleValue;
    }
     
    public int[] Cells
    {
        get
        {
            return _cells;
        }
    }

    public int AngleValue
    {
        get
        {
            return _angleValue;
        }
    }

    public void Print(){
        Debug.Log("angleValue : " + _angleValue + "/  " + string.Join(", ", _cells));
    }
}