using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[CreateAssetMenu(fileName = "StateData.asset", menuName = "gbros/StateData", order = 1)]
#endif
public class StateData : ScriptableObject{

    [SerializeField]
    private List<State> _stateList;

    private List<Type> _preloadStateTypes = new List<Type>() { typeof(SelectThemeState), typeof(SelectStageState), typeof(CollectionBookState), typeof(TitleState) };

    public List<State> StateList { get { return _stateList; } }
    public List<Type> PreloadStateTypes { get { return _preloadStateTypes; } }
}
