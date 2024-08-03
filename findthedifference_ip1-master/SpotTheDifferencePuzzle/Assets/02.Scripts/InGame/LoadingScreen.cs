using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class LoadingScreen : MonoBehaviour
{
    const float SCREEN_APPEAR_TIME_RATIO = 0.3f;

    [Header("Screens")]
    [SerializeField] CanvasGroup _screen;
    
    public void OnInitialize()
    {
        gameObject.SetActive(false);
    }

    public void OnShow(float delay)
    {

        StateManager.Instance.ShowSpinner();

        gameObject.SetActive(true);
        _screen.alpha = 0f;

        LeanTween.value(0f, 1f, delay).setOnUpdate(v =>        
        {
            _screen.alpha = v;
        });

    }

    public void OnHide(float delay)
    {

        StateManager.Instance.HideSpinner();

        LeanTween.value(1f, 0f, delay).setOnUpdate(v =>
        {
            _screen.alpha = v;
            gameObject.SetActive(false);
        });

    }
}