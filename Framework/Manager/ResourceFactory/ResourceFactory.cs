#if ADDRESSABLE_SUPPORT
using UnityEngine.AddressableAssets;
#endif
#if VCONTAINER_SUPPORT
using VContainer.Unity;
#endif
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Mine.Code.Framework.Manager.ResourceFactory
{
    public class ResourceFactory<T> where T : Object
    {
        #region Inner Classes
        
        public class ResourceFactoryBuilder : ResourceFactoryBuilderBase<ResourceFactoryBuilder>
        {
            public ResourceFactory<T> Build(string path)
            {
                var resourceFactory = new ResourceFactory<T>(
                    path: path,
                    isAddressable: isAddressable,
                    isInject: isInject
                );

                return resourceFactory;
            }
        }

        #endregion

        #region Fields

        readonly bool isInject;

        readonly bool isAddressable;
        protected readonly string path;

        #endregion

        #region Constructor

        protected ResourceFactory(string path, bool isAddressable, bool isInject)
        {
            this.path = path;
            this.isAddressable = isAddressable;
            this.isInject = isInject;
        }

        #endregion

        #region Static Methods

        public static ResourceFactoryBuilder Builder => new ResourceFactoryBuilder();

        #endregion

        #region Virtual Methods

        public virtual async UniTask<T> LoadAsync(Transform parent = null)
        {
            T resource = null;
            
            if (isAddressable)
            {
#if ADDRESSABLE_SUPPORT
                resource = await Addressables.LoadAssetAsync<T>(path);
#endif
            }
            else resource = await Resources.LoadAsync<T>(path) as T;

            if(!resource) return null;
            
            if (typeof(Component).IsAssignableFrom(typeof(T)) || typeof(GameObject).IsAssignableFrom(typeof(T)))
            {
                T instance = null;
                if (isInject)
                {
#if VCONTAINER_SUPPORT
                    instance = VContainerSettings.Instance.RootLifetimeScope.Container.Instantiate(resource);
#endif
                }
                else instance = Object.Instantiate(resource, parent);
                
                instance.name = instance.name.Replace("(Clone)", string.Empty);
                
#if ADDRESSABLE_SUPPORT
                if (instance is Component component)
                    component.OnDestroyAsObservable().Where(_ => isAddressable).Subscribe(_ => Addressables.Release(resource));
                else if (instance is GameObject gameObject) 
                    gameObject.OnDestroyAsObservable().Where(_ => isAddressable).Subscribe(_ => Addressables.Release(resource));
#endif
                return instance;
            }

            return resource;
        }

        public virtual void Release(T resource)
        {
            if (resource is Component component)
                Object.Destroy(component.gameObject);
            else if (resource is GameObject gameObject)
                Object.Destroy(gameObject);
            else
            {
#if ADDRESSABLE_SUPPORT
                if(isAddressable) Addressables.Release(resource);
#endif
            }
        }

        #endregion
    }
}