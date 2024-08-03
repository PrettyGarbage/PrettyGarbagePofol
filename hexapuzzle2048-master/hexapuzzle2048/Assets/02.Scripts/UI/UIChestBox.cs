using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIChestBox : MonoBehaviour {

	[SerializeField]
	private Animator _boxAnimator;

	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	void Start()
	{
		
	}

	public void PlayIdle(){
		_boxAnimator.Play("idle", -1, 0);
	}

	public void PlayOpenBox(){
		//SoundManager.Instance.PlayUISound(gameObject, GameConstants.UIFX_CHESTBOX_OPEN);
		_boxAnimator.Play("chest_box_open", -1, 0);
	}

	public void PlayCloseBox()
	{
		_boxAnimator.Play("chest_box_close", -1, 0);
	}

	public void PlayOpenBox2()
	{
		_boxAnimator.Play("chest_box_open2", -1, 0);
	}
}
