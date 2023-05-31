using UniRx.Toolkit;
using UnityEngine;

namespace MJ.Common.ObjectPool
{
    public class Pool : ObjectPool<Poolable>
    {
        private readonly Poolable _prefab;
        private readonly Transform _parent;

        public Pool(Poolable prefab, Transform parent)
        {
            _prefab = prefab;
            _parent = parent;
        }
        
        protected override Poolable CreateInstance()
        {
            var instance = Object.Instantiate(_prefab, _parent);
            instance.gameObject.SetActive(false);
            return instance;
        }
    }
}