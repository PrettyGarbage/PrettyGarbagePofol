#if VCONTAINER_SUPPORT
using VContainer;
using VContainer.Unity;

namespace Mine.Code.Framework.Template
{
    public class VObject<T> where T : LifetimeScope
    {
        #region Properties
    
        [Inject] protected LifetimeScope context { private get; set; }

        protected T Context => context as T;

        #endregion
    }
}

#endif