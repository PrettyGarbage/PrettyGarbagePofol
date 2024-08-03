using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class UICombo : UIMessage
{
	[Header("Combo")]
	[SerializeField] Sprite[] _numberSprites;
	[SerializeField] Image _numberImage;
	
	List<Image> _numberImageBuffer = new List<Image>();

	public override void Initialize()
	{
		base.Initialize();

		_numberImageBuffer.Add(_numberImage);
	}

	public override void Show(params object[] args)
	{
		if (args.Length <= 0 || args[0].GetType() != typeof(int))
		{
			return;
		}

		int combo = (int)args[0];

		string comboString = combo.ToString();

		CheckNumberImageBuffer(comboString.Length);

		for (int i = comboString.Length - 1, j = 0; i >= 0; --i, ++j)
		{
			int number = int.Parse(comboString.Substring(i, 1));

			_numberImageBuffer[j].gameObject.SetActive(true);
			_numberImageBuffer[j].transform.SetAsFirstSibling();
			_numberImageBuffer[j].sprite = _numberSprites[number];
			_numberImageBuffer[j].SetNativeSize();
		}

		for (int i = comboString.Length; i < _numberImageBuffer.Count; ++i)
		{
			_numberImageBuffer[i].gameObject.SetActive(false);
		}

		base.Show(args);
	}

	void CheckNumberImageBuffer(int length)
	{
		for (int i = _numberImageBuffer.Count; i < length; ++i)
		{
			Image numberImage = Instantiate(_numberImage);
			numberImage.transform.SetParent(transform);
			numberImage.transform.localScale = Vector3.one;
			_numberImageBuffer.Add(numberImage);
		}
	}
}