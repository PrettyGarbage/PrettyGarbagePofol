using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ListUtil
{

    public static List<T> RandomSort<T>(List<T> list)
    {
        Random rand = new Random();
        return list.OrderBy(c => rand.Next()).ToList();
    }

    public static int GetRandomRatioId(int[] ids, int[] ratios)
    {
        int retId = 0;
        List<int> accumulationRatioList = new List<int>();
        int accumulationValue = 0;
        for (int i = 0; i < ids.Length; i++)
        {
            if (ids[i] == 0)
            {
                accumulationRatioList.Add(0);
            }
            else
            {
                accumulationValue += ratios[i];
                accumulationRatioList.Add(accumulationValue);
            }
        }

        int randomValue = UnityEngine.Random.Range(1, accumulationValue);

        for (int i = 0; i < accumulationRatioList.Count; i++)
        {
            if (accumulationRatioList[i] >= randomValue && accumulationRatioList[i] != 0)
            {
                retId = ids[i];
                break;
            }
        }

        return retId;


    }
}