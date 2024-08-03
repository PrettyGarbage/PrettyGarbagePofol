using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupLangSelectState : PopupState {

	public static void Open()
    {
        StateManager.Instance.OpenPopupState<BaseStateData>(typeof(PopupLangSelectState));
    }
}
