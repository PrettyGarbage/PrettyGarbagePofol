using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestWebview : MonoBehaviour {

    [SerializeField]
    private RectTransform _webViewRect;

    [SerializeField]
    private TMP_Text _logText;

    private UniWebView _webView;

    public void OnView()
    {
        _logText.text = "OnView";

        if (_webView == null)
        {
            _webView = gameObject.AddComponent<UniWebView>();
        }

        _webView.ReferenceRectTransform = _webViewRect;
        _webView.Load("https://www.naver.com/");
        _webView.Show();
        
        _webView.OnPageFinished += (view, statusCode, url) => {
            _logText.text = "OnPageFinished";            
        };

        _webView.OnShouldClose += (view) => {
            _logText.text = "OnShouldClose";
            CloseWebView();
            return true;
        };

    }

    public void OnBack()
    {
        if (_webView != null)
            _webView.InternalOnShouldClose();
    }

    void CloseWebView()
    {
        if (_webView != null)
        {
            Destroy(_webView);
            _webView = null;
        }
    }

    void OnDestroy()
    {
        CloseWebView();
    }

}
