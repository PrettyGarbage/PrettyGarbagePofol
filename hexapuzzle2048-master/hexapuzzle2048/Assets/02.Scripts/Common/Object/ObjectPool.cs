using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectPool<T>
{
    //private string _pathPrefix;
    //private Resource_Type _resourceType;

    private Dictionary<T, Stack<GameObject>> _objectDic;

    public ObjectPool()
    {
        _objectDic = new Dictionary<T, Stack<GameObject>>();
    }

    //private string GetPrefabPath(T key)
    //{
    //    Debug.Log("GetPrefabPath Key : " + key.ToString());
    //    string cardName = DataManager.Instance.GetCharacterInfo(unitNo).name;
    //    return _pathPrefix + cardName;
    //}

    public int GetObjectCount(T key)
    {
        Stack<GameObject> stack;
        if (_objectDic.TryGetValue(key, out stack))
        {
            return stack.Count;
        }
        return 0;
    }

    public GameObject GetObject(T key)
    {
        Stack<GameObject> stack;
        if (_objectDic.TryGetValue(key, out stack))
        {
            if (stack.Count > 0)
            {
                GameObject retObject = stack.Pop();
                //Debug.LogWarning("GetObject Stack key :" + key.ToString() + "/ cardStack count: " + stack.Count);
                return retObject;
            }
        }

        //Debug.LogWarning("GetObject : No Pop Object");

        return null;
    }

    public void ReleaseObject(T key, GameObject cardObject)
    {
        Stack<GameObject> cardStack;

        if (_objectDic.TryGetValue(key, out cardStack))
        {
            if (!cardStack.Contains(cardObject))
            {
                cardStack.Push(cardObject);
            }
        }
        else
        {
            cardStack = new Stack<GameObject>();
            cardStack.Push(cardObject);
            _objectDic[key] = cardStack;
        }

        //Debug.LogWarning("ReleaseObject key :" + key.ToString()+ "/ stack count: " + cardStack.Count);
    }

}