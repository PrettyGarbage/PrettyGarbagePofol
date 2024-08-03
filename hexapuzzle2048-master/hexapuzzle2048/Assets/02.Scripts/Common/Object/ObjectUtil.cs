using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class ObjectUtil
{
    public static string SetObjectName(Object obj)
    {
        return obj.name = obj.name;
    }

    /// <summary>
    /// 오브젝트 생성.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="point"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public static Object Instantiate(GameObject obj, Vector3 point, Quaternion rotation)
    {
        Object retObj = Object.Instantiate(obj, point, rotation);
        SetObjectName(retObj);
        return retObj;
    }

    /// <summary>
    /// 오브젝트 생성
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static Object Instantiate(GameObject obj)
    {
        Object retObj = Object.Instantiate(obj);
        SetObjectName(retObj);
        return retObj;
    }

    public static Object InstantiateAtTarget(GameObject obj, Transform Target)
    {
        Object retObj = Object.Instantiate(obj, Target);
        SetObjectName(retObj);
        return retObj;
    }

    public static T InstantiateResourceLoad<T>(string resourcePath) where T : Component
    {
        //Debug.ForceLog("resourcePath : " + resourcePath);
        return Object.Instantiate(ResourceLoad<T>(resourcePath)) as T;
    }

    public static T ResourceLoad<T>(string resourcePath) where T : Component
    {
        return Resources.Load(resourcePath, typeof(T)) as T;
    }

    /// <summary>
    /// 로컬 Transform Zero
    /// </summary>
    /// <param name="obj"></param>
    public static void ResetZeroTransform(Transform obj)
    {
        obj.localPosition = Vector3.zero;
        obj.localScale = Vector3.one;
        obj.localRotation = Quaternion.Euler(0, 0, 0);
    }
    /// <summary>
    /// 오브젝트 자식화 및 transform 초기화.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="Parent"></param>
    public static void MoveToTransformParent(Transform obj, Transform Parent)
    {
        obj.SetParent(Parent);
        ResetZeroTransform(obj);
    }
    /// <summary>
    /// 로컬 Transform target값으로 변경.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="target"></param>
    public static void MoveToTransform(Transform obj, Transform target)
    {
        obj.localPosition = target.position;
        obj.localRotation = target.rotation;
    }
    /// <summary>
    /// 지정 point, angle 값으로 변경.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="point"></param>
    /// <param name="angle"></param>
    public static void MoveToPoint(Transform obj, Vector3 point, Vector3 angle)
    {
        obj.localPosition = point;
        obj.localEulerAngles = angle;
    }
    /// <summary>
    /// 레이어 변경 (Recursive)
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="layerNo"></param>
    public static void SetLayersRecursive(GameObject obj, int layerNo, int exLayerNo)
    {
        if (obj.layer != exLayerNo) obj.layer = layerNo;

        foreach (Transform child in obj.transform)
        {
            SetLayersRecursive(child.gameObject, layerNo, exLayerNo);
        }
    }

    public static void SetTagRecursive(GameObject obj, string tag)
    {
        obj.tag = tag;

        foreach (Transform child in obj.transform)
        {
            SetTagRecursive(child.gameObject, tag);
        }
    }
    /// <summary>
    /// Renderer 컬러 변경.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="color"></param>
    /// <param name="property"></param>
    public static void ChangeAllMeshRendererColor(GameObject obj, Color color, string property = "")
    {
        Renderer[] rendererArray = obj.GetComponentsInChildren<Renderer>();
        if (rendererArray != null)
        {
            for (int i = 0; i < rendererArray.Length; i++)
            {
                ChangeMeshRendererColor(rendererArray[i], color, property);
            }
        }
    }

    public static void ChangeMeshRendererColor(Renderer renderer, Color color, string property = "")
    {
        if (property.Equals(""))
        {
            renderer.material.color = color;
        }
        else
        {
            if (renderer.material.HasProperty(property))
            {
                renderer.material.SetColor(property, color);
            }
        }
    }

    public static void ChangeMeshRendererColor(Renderer renderer, int matIndex, Color color, string property = "")
    {
        if (renderer.materials.Length < matIndex) return;
        Material mat = renderer.materials[matIndex];

        if (property.Equals(""))
        {
            mat.color = color;
        }
        else
        {
            if (mat.HasProperty(property))
            {
                mat.SetColor(property, color);
            }
        }
    }

    public static T GetShortDistanceObject<T>(Transform objTransform, Component[] gameObjects) where T : Component
    {
        return GetShortDistancePoint<T>(objTransform.position, gameObjects);
    }

    public static T GetShortDistancePoint<T>(Vector3 point, Component[] gameObjects) where T : Component
    {
        float shortDistance = 1000f;
        T retObject = default(T);
        for (int i = 0; i < gameObjects.Length; i++)
        {
            float distance = (point - gameObjects[i].transform.position).sqrMagnitude;
            if (distance < shortDistance)
            {
                shortDistance = distance;
                retObject = gameObjects[i].GetComponent<T>();
            }
        }

        return retObject;
    }

    public static T GetShortDistancePoint<T>(Vector3 point, List<T> gameObjectList, int exceptInstaceId) where T : Component
    {
        float shortDistance = 1000f;
        T retObject = default(T);
        for (int i = 0; i < gameObjectList.Count; i++)
        {
            if (gameObjectList[i])
            {

                if (gameObjectList[i].GetInstanceID() == exceptInstaceId)
                    continue;

                float distance = (point - gameObjectList[i].transform.position).sqrMagnitude;
                if (distance < shortDistance)
                {
                    shortDistance = distance;
                    retObject = gameObjectList[i];
                }
            }
            
        }

        return retObject;
    }

    public static Vector3 GetLongDistancePoint(Vector3 originPoint, List<Vector3> pointList)
    {
        float longDistance = 0f;
        Vector3 retPoint = Vector3.zero;
        for (int i = 0; i < pointList.Count; i++)
        {
            float distance = (originPoint - pointList[i]).sqrMagnitude;
            if (distance > longDistance)
            {
                longDistance = distance;
                retPoint = pointList[i];
            }
        }

        return retPoint;
    }

    public static T GetShortDistancePoint<T>(Vector3 point, GameObject[] gameObjects)
    {
        float shortDistance = 1000f;
        T retObject = default(T);
        for (int i = 0; i < gameObjects.Length; i++)
        {
            //float distance = Vector3.Distance(point, item.transform.position);
            float distance = (point - gameObjects[i].transform.position).sqrMagnitude;
            if (distance < shortDistance)
            {
                shortDistance = distance;
                retObject = gameObjects[i].GetComponent<T>();
            }
        }

        return retObject;
    }

    public static List<T> GetComponentsInRecursive<T>(GameObject obj, bool isExcludeParent = true) where T : Component
    {
        List<T> retList = new List<T>();
        T[] objectArray = obj.GetComponentsInChildren<T>();
        for (int i = 0; i < objectArray.Length; i++)
        {
            if (isExcludeParent && obj.GetInstanceID() == objectArray[i].gameObject.GetInstanceID())
            {
                continue;
            }
            retList.Add(objectArray[i]);
        }

        return retList;
    }

    public static void ChangeRendererMaterial(GameObject obj, Material material)
    {
        Renderer[] rendererArray = obj.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < rendererArray.Length; i++)
        {
            rendererArray[i].material = material;
        }
    }
    /// <summary>
    /// obj Y 기준 target 위치.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static Vector3 GetPointByObjectUp(Transform obj, Transform target)
    {
        return new Vector3(target.position.x, obj.position.y, target.position.z);
    }

    public static Vector3 GetPointGround(Vector3 point, float y)
    {
        return new Vector3(point.x, y, point.z);
    }

    public static void ObjectInfoDebug(System.Object obj)
    {
        FieldInfo[] FieldInfoArray = obj.GetType().GetFields();
        if (FieldInfoArray != null && FieldInfoArray.Length > 0)
        {
            StringBuilder sb = new StringBuilder(FieldInfoArray.Length);
            for (int i = 0; i < FieldInfoArray.Length; i++)
            {
                sb.Append(FieldInfoArray[i].Name + " : " + FieldInfoArray[i].GetValue(obj));
                sb.AppendLine();
            }

            Debug.Log(sb.ToString());
        }
    }

    public static T CreateInstance<T>(string name) where T : Component
    {
        T instance = GameObject.FindObjectOfType<T>();

        if (instance == null)
            instance = new GameObject(name).AddComponent<T>();

        return instance;
    }

    public static bool IsLeftPosition(Transform origin, Vector3 targetPoint)
    {
        return Vector3.Dot((targetPoint - origin.position).normalized, origin.right) < 0 ? true : false ;
        
    }

    public static Vector3 GetCenterPositiom(Component[] components)
    {
        Vector3 centerPoint = Vector3.zero;
        int count = 0;
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i])
            {
                centerPoint += components[i].transform.position;
                count++;
            }
        }

        return count ==0 ? Vector3.zero : centerPoint / count;
    }

    public static Vector3 GetCenterPositiom<T>(List<T> componentList) where T : Component
    {
        Vector3 centerPoint = Vector3.zero;
        int count = 0;
        for (int i = 0; i < componentList.Count; i++)
        {
            if (componentList[i] != null)
            {
                centerPoint += componentList[i].transform.position;
                count++;
            }
        }

        return count == 0 ? Vector3.zero : centerPoint / count;
    }

    public static byte[] ToByteArray<T>(T obj)
    {
        if(obj == null)
            return null;
        BinaryFormatter bf = new BinaryFormatter();
        using(MemoryStream ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    public static T FromByteArray<T>(byte[] data)
    {
        if(data == null)
            return default(T);
        BinaryFormatter bf = new BinaryFormatter();
        using(MemoryStream ms = new MemoryStream(data))
        {
            object obj = bf.Deserialize(ms);
            return (T)obj;
        }
    }
}