using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class ScenarioEventProduction : MonoBehaviour
{
    #region Properties

    [field: SerializeField] public Dialogue[] Dialogues { get; private set; }
    public string EventCode { get; set; }

    #endregion
    
    #region Abstract Methods

    public virtual async UniTask OnPrevStartMission(bool isObserver) { }
    public virtual void OnAfterFinishMission(bool isObserver) { }

    #endregion
}
