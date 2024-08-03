using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ThemeGameOverStateData : BaseStateData
{
    public int totalDifferenceCount;
    public int findDifferenceCount;

    public ThemeGameOverStateData() { }
    public ThemeGameOverStateData(int totalDifferenceCount, int findDifferenceCount)
    {
        this.totalDifferenceCount = totalDifferenceCount;
        this.findDifferenceCount = findDifferenceCount;
    }
}

public class ThemeGameOverState : NotBackablePopupState
{

    [SerializeField] private TMP_Text _getStarText;
    [SerializeField] private TMP_Text _maxStarText;

    public override IEnumerator OnPreOpen<T>(T args = null)
    {
        yield return base.OnPreOpen(args);

        ThemeGameOverStateData themeGameOverStateData = GetData<ThemeGameOverStateData>();

        _getStarText.text = themeGameOverStateData.findDifferenceCount.ToString();

        _maxStarText.text = "/ " + themeGameOverStateData.totalDifferenceCount;

    }

    #region EVENTS

    public void OnGoToStageList()
    {
        StateManager.Instance.OpenState<BaseStateData>(typeof(SelectStageState), true);
    }

    public void OnGoToThemeList()
    {
        StateManager.Instance.OpenState<BaseStateData>(typeof(SelectThemeState), true);
    }

    public void OnRetry()
    {
        StateManager.Instance.OpenState<BaseStateData>(typeof(PopupReadyState), true);
    }
    #endregion EVENTS
}