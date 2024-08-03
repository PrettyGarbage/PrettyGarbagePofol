using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curtain : MonoBehaviour
{
	[SerializeField] RectTransform _root;
	[SerializeField] float _sideOffset = 300.0f;

	public void Show()
	{
		_root.LeanCancel();
		_root.localPosition = Vector3.zero;
	}

	public void Hide()
	{
		_root.LeanCancel();
		_root.localPosition = Vector3.left * (_root.sizeDelta.x + _sideOffset * 2.0f);
	}

    public void ShowFrom(CurtainDirection from, float time)
    {
        int direction = (int)from;

        Vector3 position = Vector3.zero;
        position.x = -direction * (_root.sizeDelta.x + _sideOffset * 2.0f);
        _root.localPosition = position;

        _root.LeanCancel();
        LeanTween.value(time, 0.001f, 0.4f)
            .setOnUpdate((float val) =>
            {
                _root.LeanMoveLocalX(0.0f, val);
            }).setOnComplete(()=>
            {
                _root.LeanCancel();
            }); ;
	}

	public void HideTo(CurtainDirection to, float time)
    {
        int direction = (int)to;

        _root.localPosition = Vector3.zero;

		_root.LeanCancel();
		_root.LeanMoveLocalX(direction * (_root.sizeDelta.x + _sideOffset * 2.0f), time);
    }

}