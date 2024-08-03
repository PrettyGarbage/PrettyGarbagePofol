using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WebViewManager : MonoBehaviour {

    const string QNA_UID = "uid";
    const string QNA_OS = "os";
    const string QNA_VERSION = "version";

    private static WebViewManager _instance;
    public static WebViewManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = ObjectUtil.CreateInstance<WebViewManager>("WebViewManager");
            }
            return _instance;
        }
    }

    [SerializeField]
    private GameObject _webViewObject;

    [SerializeField]
    private RectTransform _webViewRect;
    [SerializeField]
    private SimpleSpinner _simpleSpinner;

    private UniWebView _webView;

    [SerializeField]
    private Button _qnaButton;

    private void Awake()
    {
        _webViewObject.SetActive(false);
        _qnaButton.onClick.AddListener(OnQna);
    }

    public void Show(string targetUrl)
    {
        Debug.Log("WebViewManager Show " + targetUrl);

        _qnaButton.gameObject.SetActive(true);
        _webViewObject.SetActive(true);

        _simpleSpinner.Show();

        targetUrl = targetUrl + GetRequestParam();

        LoadWebView(targetUrl);

    }

    private void LoadWebView(string targetUrl, string html="")
    {
        if (_webView == null)
        {
            _webView = gameObject.AddComponent<UniWebView>();
        }

        _webView.ReferenceRectTransform = _webViewRect;
        if (html.Length > 0)
        {
            _webView.LoadHTMLString(html, targetUrl);
        }
        else
        {
            _webView.Load(targetUrl);
        }
        _webView.Show();

        _webView.OnPageFinished += (view, statusCode, url) => {
            Debug.Log("WebViewManager OnPageFinished : " + url + "/ statusCode : " + statusCode);
            _simpleSpinner.Hide();
        };

        _webView.OnShouldClose += (view) => {
            Debug.Log("WebViewManager OnShouldClose");
            CloseWebView();
            _webViewObject.SetActive(false);
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

    public string GetRequestParam()
    {
        string requestParam = string.Empty;
        switch (SettingManager.Instance.LangType)
        {
            case LangType.KR:
                requestParam = "?l=ko";
                break;
            case LangType.JP:
                requestParam = "?l=ja";
                break;
            case LangType.EN:
                requestParam = "?l=en";
                break;
            default:
                requestParam = "?l=en";
                break;
        }

        return requestParam;
    }

    public void OnQna()
    {
        _qnaButton.gameObject.SetActive(false);
        CloseWebView();
        StartCoroutine(LoadQnaHtml());
    }

    IEnumerator LoadQnaHtml()
    {

        _simpleSpinner.Show();

        string requestParam = GetRequestParam();
        string targetUrl = DataManager.Instance.AppInfo.qnaUrl + requestParam;

        // 통신에 저장
        WWWForm form = new WWWForm();
        form.AddField(QNA_UID, DataManager.Instance.UserInfo.Id);
        form.AddField(QNA_OS, SystemInfo.operatingSystem);
        form.AddField(QNA_VERSION, BuildManager.Instance.GetVersion());

        using (var w = UnityWebRequest.Post(targetUrl, form))
        {
            yield return w.SendWebRequest();

            _simpleSpinner.Hide();

            if (w.isNetworkError || w.isHttpError)
            {
                CloseWebView();
                _webViewObject.SetActive(false);
                MessageBoxState.Open(new GameErrorInfo(ApiErrorCode.BAD_REQUEST));
            }
            else
            {
                LoadWebView(targetUrl, w.downloadHandler.text);
            }
        }

    }

}
