using UniRx;
using UnityEngine;

[CreateAssetMenu(fileName = "New RxObject", menuName = "ScriptableObjects/RxObject")]
public class RxObject : ScriptableObject
{
    #region Properties

    public Subject<Unit> OnEvent { get; } = new();

    #endregion
}
