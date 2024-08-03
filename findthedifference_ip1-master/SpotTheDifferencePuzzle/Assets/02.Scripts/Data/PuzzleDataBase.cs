using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[CreateAssetMenu(fileName = "PuzzleDataBase.asset", menuName = "gbros/PuzzleDataBase", order = 1)]
#endif
public class PuzzleDataBase : ScriptableObject{

	[SerializeField]
	private PuzzleModeData _themeBasicModeData;
	[SerializeField]
	private PuzzleModeData _themeMasterModeData;
   
    public PuzzleModeData ThemeBasicModeData { get { return _themeBasicModeData; } }
    public PuzzleModeData ThemeMasterModeData { get { return _themeMasterModeData; } }

}
 