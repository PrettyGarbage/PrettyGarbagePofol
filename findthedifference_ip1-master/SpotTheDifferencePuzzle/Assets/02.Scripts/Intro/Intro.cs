using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
	[Header("Logo")]
	[SerializeField] Graphic _gbros;
	[SerializeField] float _logoDuration = 1.0f;

	[Header("Loading Spinner")]
	[SerializeField]
    private Slider _lodingSlider;
    

	void Awake()
	{	
        _lodingSlider.value = 0;
    }

	IEnumerator Start()
	{
        yield return new WaitForSeconds(0.1f);

		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(GameConstants.SCENE_MAIN, LoadSceneMode.Single);

        while (!asyncOperation.isDone)
        {
            _lodingSlider.value = asyncOperation.progress;
            yield return null;
        }

        _lodingSlider.value = 1f;

    }
}
;