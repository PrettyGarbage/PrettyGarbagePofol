using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupGoldInfoState : PopupState {

    [SerializeField] TMP_Text _totalGoldValue;
    [SerializeField] TMP_Text _paidGoldValue;
    [SerializeField] TMP_Text _freeGoldValue;

    public static void Open()
    {
        StateManager.Instance.OpenPopupState<BaseStateData>(typeof(PopupGoldInfoState));
    }

    public override IEnumerator OnPreOpen<T>(T args = null)
    {
        UserInfo userInfo = DataManager.Instance.UserInfo;

        _totalGoldValue.text = SystemUtil.GetCommaText((userInfo.PaidGold + userInfo.FreeGold));
        _paidGoldValue.text = SystemUtil.GetCommaText(userInfo.PaidGold);
        _freeGoldValue.text = SystemUtil.GetCommaText(userInfo.FreeGold);

        return base.OnPreOpen(args);

    }
}
