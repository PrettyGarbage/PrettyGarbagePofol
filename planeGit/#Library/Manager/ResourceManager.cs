using System;
using System.Collections.Generic;
using MJ.Common.ObjectPool;
using MJ.Utils;
using UniRx.Toolkit;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Manager
{
    public class ResourceManager
    {
        #region Variables

        private readonly Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();
        private readonly Dictionary<string, Transform> _poolsTransforms = new Dictionary<string, Transform>();

        #endregion
        
        #region Public Methods

        public void Initialize()
        {
            Logger.Log("Init ResourceManager", Logger.LogLevel.Resource);
        }
        
        //타입에 따른 리소스 로드
        private static T Load<T>(string path) where T : Object
        {
            Logger.Log("Path : " + path, Logger.LogLevel.Data);
            
            return Resources.Load<T>(path);
        }
        
        //Instantiate하고 GameObject를 리턴하는 함수
        public GameObject Instantiate(string path, Transform parent = null, bool isPoolingObj = false)
        {
            if (!Application.isPlaying) return null;
            
            Logger.Log(path);
            
            var origin = Load<GameObject>($"Prefabs/{path}");

            if (origin == null)
            {
                Logger.Log($"Failed to load prefab: {path}", Logger.LogLevel.Data);
                return null;
            }

            //풀링 오브젝트라면 오브젝트 풀링 기법을 이용하여 처리한다.
            if (isPoolingObj)
            {
                var poolObj = origin.GetOrAddComponent<Poolable>();
                poolObj.isUsing = true;

                Pool pool = null;
                GameObject rentObj = null;
                var poolName = "Pool - " + poolObj.name;

                if (!_pools.ContainsKey(poolObj.gameObject.name))
                {
                    var category = new GameObject(poolName).transform;
                    category.SetParent(parent);
                    _poolsTransforms.Add(poolName, category); 
                    pool = new Pool(poolObj, parent);
                    _pools.Add(poolObj.gameObject.name, pool);
                    rentObj = pool.Rent().gameObject;
                }
                else
                    rentObj = _pools[poolObj.gameObject.name].Rent().gameObject;

                
                rentObj.transform.SetParent(_poolsTransforms[poolName]);

                return rentObj;
            }

            var go = Object.Instantiate(origin, parent);
            go.name = origin.name;
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            
            return go;
        }
        
        public void Destroy(GameObject go)
        {
            if (!go) return;

            var poolObj = go.GetComponent<Poolable>();
            
            if (!poolObj)
            {
                Object.Destroy(go);
                return;
            }

            //복사할때 (Clone)이라는 단어가 붙는거 제거하고 검색해야 됨.
            int indexOfClone = poolObj.gameObject.name.IndexOf("(Clone)", StringComparison.Ordinal);
            string objKeyValue = indexOfClone > 0 ? poolObj.gameObject.name.Substring(0, indexOfClone) : poolObj.gameObject.name;

            if(_pools.ContainsKey(objKeyValue))
                _pools[objKeyValue].Return(poolObj);
            else
                Logger.Log("Failed to return poolable object", Logger.LogLevel.Error);
        }

        #endregion
        
    }
}