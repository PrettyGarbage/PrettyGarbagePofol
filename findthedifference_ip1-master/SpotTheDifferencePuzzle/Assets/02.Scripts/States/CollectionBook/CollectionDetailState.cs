using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class CollectionDetailStateData : BaseStateData
{
    public CollectionInfo collectionInfo;
    public Color gradeColor;
}

public class CollectionDetailState : PopupState
{
	[Header("CollectionDetailState")]
	[SerializeField] Image _image;
    [SerializeField] Image _borderImage;

    [SerializeField]
    private float _borderLengthRatio = 5;
    private float _borderLength;

    public static void Open(CollectionInfo collectionInfo, Color gradeColor)
    {
        CollectionDetailStateData collectionDetailStateData = new CollectionDetailStateData();
        collectionDetailStateData.collectionInfo = collectionInfo;
        collectionDetailStateData.gradeColor = gradeColor;
        StateManager.Instance.OpenPopupState<CollectionDetailStateData>(typeof(CollectionDetailState), collectionDetailStateData);
    }
	
	public override IEnumerator OnPreOpen<T>(T args = default(T))
    {
		yield return base.OnPreOpen(args);
        _image.gameObject.SetActive(false);

        Resolution resolution = Screen.currentResolution;
        _borderLength = resolution.height * _borderLengthRatio / 100;

        CollectionDetailStateData collectionDetailStateData = GetData<CollectionDetailStateData>();
        _borderImage.color = collectionDetailStateData.gradeColor;

        StateManager.Instance.ShowSpinner();
        DataManager.Instance.GetCollectionImage(collectionDetailStateData.collectionInfo.id, (sprite) => {

            StateManager.Instance.HideSpinner();

            _image.gameObject.SetActive(true);
            _image.sprite = sprite;

            float width = sprite.texture.width;
            float height = sprite.texture.height;

            float borderX = resolution.width - width;
            float borderY = resolution.height - height;

            float targetHeight;
            float targetWidth;

            if (borderX > borderY)
            {
                targetHeight = resolution.height - (_borderLength * 2);
                targetWidth = width * (targetHeight / height);
            }
            else
            {
                targetWidth = resolution.width - (_borderLength * 2);
                targetHeight = height * (targetWidth / width);
            }

            _image.rectTransform.sizeDelta = new Vector2(targetWidth, targetHeight);

            _image.rectTransform.localPosition = Vector3.zero;


            Debug.Log("width : " + _image.GetPixelAdjustedRect().width);
            Debug.Log("height : " + _image.GetPixelAdjustedRect().height);

        });
    }

    public override IEnumerator OnClosing()
    {
        _image.sprite = null;
        return base.OnClosing();
    }

}