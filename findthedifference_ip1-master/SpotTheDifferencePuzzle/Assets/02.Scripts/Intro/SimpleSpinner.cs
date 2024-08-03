using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSpinner : MonoBehaviour {

    [SerializeField]
    private float _speed = 1f;
    [SerializeField]
    private float _fadeTime = 0.5f;

    [SerializeField]
    private Transform _imageTransform;
    private CanvasGroup _canvasGroup;
    
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (_imageTransform && _canvasGroup.alpha > 0f)
        {
            _imageTransform.eulerAngles += Vector3.forward * Time.deltaTime * _speed;
        }
    }

    public void Show()
    {
        gameObject.LeanCancel();
        _canvasGroup.alpha = 0f;
        LeanTween.value(gameObject, 0f, 1f, _fadeTime).setOnUpdate(v=>{
            _canvasGroup.alpha = v;
        });

    }

    public void Hide()
    {
        gameObject.LeanCancel();
        _canvasGroup.alpha = 1f;
        LeanTween.value(gameObject, 1f, 0f, _fadeTime).setOnUpdate(v => {
            _canvasGroup.alpha = v;
        });
    }

}
