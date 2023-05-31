using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TabletCellModel
{
    [field: SerializeField] public string Description { get; private set; }
    public BoolReactiveProperty IsClear { get; } = new();
}

public class Tablet : MonoBehaviour
{
    #region Properties

    [field: SerializeField] public ScrollRect ScrollRect { get; private set; }
    [field: SerializeField] public TabletCellView CellPrefab { get; private set; }
    [field: SerializeField] public Transform Viewport { get; private set; }
    [field: SerializeField] public Transform CellParent { get; private set; }
    [field: SerializeField] public List<TabletCellModel> Cells { get; private set; }

    float CellHeight => CellPrefab.GetComponent<RectTransform>().rect.height;
    float ScrollRectHeight => ScrollRect.GetComponent<RectTransform>().rect.height;
    float MaxCellCount => ScrollRectHeight / CellHeight;

    #endregion

    #region Unity Lifecycle

    void Awake()
    {
        foreach (var cell in Cells)
        {
            var cellView = Instantiate(CellPrefab, CellParent);
            cellView.MissionIndexText.text = (Cells.IndexOf(cell) + 1).ToString();
            cellView.DescriptionText.text = cell.Description;
            cell.IsClear.Subscribe(isClear => cellView.ClearIcon.SetActive(isClear)).AddTo(this);
        }
    }

    #endregion

    #region Public Methods

    public void ToggleCell(int cellIndex, bool toggle)
    {
        Cells[cellIndex].IsClear.Value = toggle;
    }
    
    public void FocusCell(int cellIndex)
    {
        var verticalScrollbarValue = 1 - ((cellIndex - MaxCellCount * 0.5f) / (Cells.Count - MaxCellCount));
        ScrollRect.verticalNormalizedPosition = verticalScrollbarValue;
    }

    #endregion
}