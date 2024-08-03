using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

public class UIMissionClear : UIMessage
{
	[Header("Mission Clear")]
	[SerializeField] TMP_Text _rewardText;

	public override void Show(params object[] args)
	{
		if (args.Length <= 0 || args[0].GetType() != typeof(int))
		{
			return;
		}

		int reward = (int)args[0];

		_rewardText.text = string.Format(
			GameConstants.MISSION_REWARD_TEXT,
			reward
		);

		base.Show(args);
	}
}