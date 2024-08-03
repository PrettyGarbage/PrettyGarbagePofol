using System.Text;
using System.Collections;

using UnityEngine;

using TMPro;

public class UIMission : MonoBehaviour
{
	[SerializeField] RectTransform _root;
	[SerializeField] float _minSize = 132.0f;
	[SerializeField] float _maxSize = 222.0f;

	[SerializeField] TMP_Text _missionText;
	[SerializeField] float _missionTextScaleInterval = 30.0f;
	[SerializeField] Vector3 _missionTextScale = Vector3.one * 1.1f;
	[SerializeField] float _missionTextScalingTime = 0.5f;

	StringBuilder _stringBuilder = new StringBuilder();

	public void Reset()
	{
		_missionText.LeanCancel();
		_missionText.transform.localScale = Vector3.one;

		gameObject.LeanCancel();
		UpdateHeight(_minSize);
	}

	public float Show(int missionIndex, float time = 0.3f)
	{
		float hideTime = Hide(time);

		GameData gameData = DataManager.Instance.GameData;
		if (missionIndex < 0 ||
			missionIndex >= gameData.MissionRewardScores.Length)
		{
			return hideTime;
		}

		gameObject.LeanValue(_minSize, _maxSize, time).
			setDelay(hideTime).
			setEaseOutBack().
			setOnStart(() => {
				_stringBuilder.Remove(0, _stringBuilder.Length);
				_stringBuilder.AppendFormat(
					GameConstants.MISSION_TARGET_TEXT,
					gameData.MissionRewardScores[missionIndex]
				);
				_missionText.text = _stringBuilder.ToString();
				SoundManager.Instance.PlayUISoundInstance(GameConstants.FX_MISSION_APPEAR);
			}).
			setOnUpdate(UpdateHeight);

		return hideTime + time;
	}

	public float Hide(float time = 0.3f)
	{
		time *= (_root.sizeDelta.y - _minSize) / (_maxSize - _minSize);

		gameObject.LeanCancel();
		gameObject.LeanValue(_root.sizeDelta.y, _minSize, time).
			setEaseInBack().
			setOnUpdate(UpdateHeight);

		return time;
	}

	public void Emphasize()
	{
		float halfTime = _missionTextScalingTime * 0.5f;

		_missionText.LeanCancel();

		LTSeq sequence = LeanTween.sequence();
		sequence.append(_missionText.LeanScale(_missionTextScale, halfTime).setEaseOutBack());
		sequence.append(_missionText.LeanScale(Vector3.one, halfTime).setEaseInBack());
	}

	void UpdateHeight(float height)
	{
		Vector2 size = _root.sizeDelta;
		size.y = height;
		_root.sizeDelta = size;
	}
}