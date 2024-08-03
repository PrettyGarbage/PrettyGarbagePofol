using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TutorialState : State
{
	[SerializeField] CanvasGroup[] _pages;
	[SerializeField] float _pageOpenTime = 1.0f;

	int _pageIndex = 0;
	CanvasGroup _page;

	public override IEnumerator OnLoad(Dictionary<string, object> args = null)
	{
		yield return base.OnLoad(args);

		_pageIndex = 0;

		for (int i = 0, max = _pages.Length; i < max; ++i)
		{
			_pages[i].gameObject.SetActive(false);

			Animator animator = _pages[i].GetComponent<Animator>();
			if (animator != null)
			{
				animator.ResetTrigger("Play");
			}
		}
	}

	public override IEnumerator OnEnter()
	{
		yield return base.OnLoad(args);

		_page = _pages[_pageIndex];

		_page.gameObject.SetActive(true);
		_page.alpha = 0.0f;

		_page.LeanAlpha(1.0f, _pageOpenTime);

		yield return new WaitForSeconds(_pageOpenTime);

		Animator animator = _page.GetComponent<Animator>();
		if (animator != null)
		{
			animator.SetTrigger("Play");
		}
	}

	public override IEnumerator OnExit()
	{
		_page.LeanCancel();
		_page.LeanAlpha(0.0f, _pageOpenTime).
			setOnComplete(() => _page.gameObject.SetActive(false));

		yield return base.OnExit();
	}

	public override string stateName
	{
		get
		{
			return GameConstants.STATENAME_TUTORIAL;
		}
	}

	public void OnNextPage()
	{
		++_pageIndex;
		if (_pageIndex >= _pages.Length)
		{
			StateManager.instance.OpenState(GameConstants.STATENAME_LOBBY);

			return;
		}

		LTSeq sequence = LeanTween.sequence();

		CanvasGroup prevPage = _page;
		if (prevPage != null)
		{
			prevPage.LeanCancel();

			sequence.append(prevPage.LeanAlpha(0.0f, _pageOpenTime * 0.5f));
			sequence.append(() => _page.gameObject.SetActive(false));
		}

		_page = _pages[_pageIndex];
		_page.alpha = 0.0f;

		sequence.append(() => _page.gameObject.SetActive(true));
		sequence.append(_page.LeanAlpha(1.0f, _pageOpenTime * 0.5f));
		sequence.append(() => {
			Animator animator = _page.GetComponent<Animator>();
			if (animator != null) {
				animator.SetTrigger("Play");
			}
		});
	}
}