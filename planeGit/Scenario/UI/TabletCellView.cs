using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabletCellView : MonoBehaviour
{
    #region Properties

    [field: SerializeField] public TextMeshProUGUI MissionIndexText { get; private set; }
    [field: SerializeField] public TextMeshProUGUI DescriptionText { get; private set; }
    [field: SerializeField] public GameObject ClearIcon { get; private set; }

    #endregion
}
