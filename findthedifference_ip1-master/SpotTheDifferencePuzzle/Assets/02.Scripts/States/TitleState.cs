using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleState : State
{
	[SerializeField] GameObject _touchScreenMessage;
	[SerializeField] float _intervalTime = 0.5f;
    [SerializeField] Button _nextButton;

	public override IEnumerator OnPreOpen<T>(T args = default(T))
    {
		yield return base.OnPreOpen(args);

		_touchScreenMessage.SetActive(true);
        _nextButton.onClick.AddListener(GoToThemeSelect);

    }

    public override IEnumerator OnPostOpen()
    {
        InvokeRepeating("MessageAction", 0f, _intervalTime);
        yield return null;
    }

    private void MessageAction()
    {
        _touchScreenMessage.SetActive(!_touchScreenMessage.activeSelf);
    }

    public void GoToThemeSelect()
	{
        _nextButton.onClick.RemoveListener(GoToThemeSelect);
        CancelInvoke("MessageAction");
        _touchScreenMessage.gameObject.SetActive(false);
        StateManager.Instance.ShowSpinner();

        if (UserInfo.IsLogined())
        {
            StateManager.Instance.OpenState<BaseStateData>(typeof(SelectThemeState), true);
        }
        else
        {
            StateManager.Instance.OpenPopupState<BaseStateData>(typeof(AccountsState));
        }
	}
}
