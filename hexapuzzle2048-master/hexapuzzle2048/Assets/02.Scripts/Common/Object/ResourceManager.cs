using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PrefabInfo
{
    public int no;
    public GameObject prefab;
}

public class ResourceManager : MonoBehaviour
{
    private static ResourceManager _instance;

    private string[] resourceTypeNameArray = { "Prefabs", "Textures", "Sounds" };

    private ObjectPool<int> _unitObjectPool;

    private ObjectPool<ResourcePoolType> _resourcePool;

    public static ResourceManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = ObjectUtil.CreateInstance<ResourceManager>("ResourceManager");
                _instance._unitObjectPool = new ObjectPool<int>();
                _instance._resourcePool = new ObjectPool<ResourcePoolType>();
                //DontDestroyOnLoad(_instance);
            }
            return _instance;
        }
    }

    /// <summary>
    /// 리스소 로드 후 생성.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    public void InstantiateResource<T>(ResourceType type, string path, ActionObject<T> callback) where T : Component
    {
        string fullPath = resourceTypeNameArray[(int)type] + "/" + path;
        Debug.Log("InstantiateResource : " + fullPath);
        StartCoroutine(InstantiateResourceCor(fullPath, callback));
    }

    IEnumerator InstantiateResourceCor<T>(string path, ActionObject<T> callback) where T : Component
    {
        T ret = ObjectUtil.InstantiateResourceLoad<T>(path);
        ObjectUtil.SetObjectName(ret);
        if (callback != null) callback(ret);

        yield return null;
    }

    public T ResourceLoad<T>(ResourceType type, string path) where T : Component
    {
        string fullPath = resourceTypeNameArray[(int)type] + "/" + path;
        Debug.Log("LoadResource : " + path);
        return ObjectUtil.ResourceLoad<T>(fullPath);
    }

    public IEnumerator CreateResource<T>(GameObject prefab, ActionObject<T> callback) where T : Component
    {
        GameObject go = ObjectUtil.Instantiate(prefab) as GameObject;
        callback(go.GetComponent<T>());
        yield return null;
    }

    public IEnumerator CreateResource<T>(GameObject prefab, Vector3 point, Quaternion rotation, ActionObject<T> callback) where T : Component
    {
        GameObject go = ObjectUtil.Instantiate(prefab, point, rotation) as GameObject;
        if (callback != null)
            callback(go.GetComponent<T>());
        yield return null;
    }

    public void GetUnit<T>(PrefabInfo unitPrefabInfo, ActionObject<T> callback) where T : Component
    {

        if (_unitObjectPool.GetObjectCount(unitPrefabInfo.no) > 0)
        {
            GameObject gObject = _unitObjectPool.GetObject(unitPrefabInfo.no);
            callback(gObject.GetComponent<T>());
        }
        else
        {
            //LoadResource<T>(Resource_Type.PREFAB, PrefabPathUtil.GetUnitPrefabPath(unitNo), callback);
            StartCoroutine(CreateResource(unitPrefabInfo.prefab, callback));
        }
    }

    public void ReleaseUnit(int unitNo, GameObject returnUnit)
    {
        _unitObjectPool.ReleaseObject(unitNo, returnUnit);
        ObjectUtil.MoveToTransformParent(returnUnit.transform, transform);
        returnUnit.SetActive(false);
    }

    public void GetResource<T>(ResourcePoolType type, GameObject prefab, ActionObject<T> callback) where T : Component
    {

        if (_resourcePool.GetObjectCount(type) > 0)
        {
            GameObject gObject = _resourcePool.GetObject(type);
            callback(gObject.GetComponent<T>());
            gObject.SetActive(true);
            //Debug.LogWarning(" GetResource1 type : " + type + " / name : " + gObject.name);
        }
        else
        {
            StartCoroutine(CreateResource(prefab, callback));
            //Debug.LogWarning(" GetResource2 type : " + type);
        }
    }

    public void ReleaseResource(ResourcePoolType type, GameObject returnUnit)
    {
        _resourcePool.ReleaseObject(type, returnUnit);
        ObjectUtil.MoveToTransformParent(returnUnit.transform, transform);
        returnUnit.SetActive(false);
        //Debug.LogWarning(" ReleaseResource type : " + type);
    }

}