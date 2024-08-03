using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
	[SerializeField] float _logoDuration = 1.0f;

	[Header("company Logo")]
	[SerializeField] Graphic _gbros;
	[SerializeField] CanvasGroup _gbrosCanvasGroup;

	
	[Header("Loading Spinner")]
	[SerializeField] GameObject _loadingSpinner;
    [SerializeField] float _roadingSpinnerSpeed = 1.0f;

	void Awake()
	{
		_gbros.CrossFadeAlpha(0.0f, 0.0f, true);
		_loadingSpinner.gameObject.SetActive(false);
		DontDestroyOnLoad(this);
	}

	IEnumerator Start()
	{
		_gbros.CrossFadeAlpha(1.0f, _logoDuration, false);
		yield return new WaitForSeconds(2f);

		LeanTween.value(1f,0f, _logoDuration).setOnUpdate((float val)=>{
			_gbrosCanvasGroup.alpha = val;
		});

		yield return new WaitForSeconds(1f);

		_loadingSpinner.SetActive(true);
		LeanTween.rotateLocal(_loadingSpinner, Vector3.forward * 720.0f, _roadingSpinnerSpeed * 2.0f).
            setLoopClamp().
            setIgnoreTimeScale(true);

		yield return StartCoroutine(CommonManager.Instance.Init());
		
		AsyncOperation aop = SceneManager.LoadSceneAsync(GameConstants.SCENE_MAIN, LoadSceneMode.Single);
		yield return new WaitForSeconds(0.5f);
		AnalyticsManager.Instance.EventLog(GameConstants.EVENTLOG_GBROS_LOGO_SHOW);

		while (!aop.isDone)
		{
			 yield return CommonConstants.WaitLoopSeconds;
		}
		yield return new WaitForSeconds(1f);
		gameObject.SetActive(false);
		Destroy(gameObject, 1f);
	}
}
