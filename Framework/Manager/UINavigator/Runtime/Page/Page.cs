#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT
using UnityEngine;

namespace Mine.Code.Framework.Manager.UINavigator.Runtime.Page
{
    public abstract class Page : UIScope
    {
        [field: SerializeField] public bool IsRecycle { get; private set; } = true;
    }
}

#endif