using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroState : State
{
	const string TUTORIAL = "Tutorial";

	public override IEnumerator OnEnter()
	{
		yield return new WaitForSeconds(1.0f);

		if (PlayerPrefs.GetInt(TUTORIAL, 0) == 0)
		{
			PlayerPrefs.SetInt(TUTORIAL, 1);

			StateManager.instance.OpenState(GameConstants.STATENAME_TUTORIAL);
		}
		else
		{
			StateManager.instance.OpenState(GameConstants.STATENAME_LOBBY);
		}
	}

	public override string stateName
	{
		get
		{
			return GameConstants.STATENAME_INTRO;
		}
	}
}