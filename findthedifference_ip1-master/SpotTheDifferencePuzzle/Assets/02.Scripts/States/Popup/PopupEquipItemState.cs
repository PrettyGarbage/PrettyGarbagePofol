using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupEquipItemState : PopupState
{

    [SerializeField]
    private Transform[] _itemTransforms;

    public static void Open()
    {
        if (GameManager.Instance.InGameData.ItemIdList.Count>0)
            StateManager.Instance.OpenPopupState<BaseStateData>(typeof(PopupEquipItemState));
    }

    public static void Close()
    {
        if (GameManager.Instance.InGameData.ItemIdList.Count > 0)
            StateManager.Instance.CloseState(typeof(PopupEquipItemState));
    }

    public override IEnumerator OnPreOpen<T>(T args = null)
    {
        yield return base.OnPreOpen(args);

        for (int i = 0; i < _itemTransforms.Length; i++)
        {
            _itemTransforms[i].gameObject.SetActive(GameManager.Instance.InGameData.ItemIdList.Contains(_itemTransforms[i].name));
        }
    }

    public override IEnumerator OnPostOpen()
    {
        if (_itemTransforms.Length > 0 )
        {
            SoundManager.Instance.PlayUISound(gameObject, AudioDataKey.ui_popup_useitem);
        }

        return base.OnPostOpen();
    }
}
