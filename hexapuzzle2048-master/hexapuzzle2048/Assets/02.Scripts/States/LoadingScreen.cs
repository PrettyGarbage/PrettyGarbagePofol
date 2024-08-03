using UnityEngine;
using UnityEngine.UI;

using TMPro;

public abstract class LoadingScreen : MonoBehaviour
{
    public abstract void OnInitialize();
    public abstract void OnShow(float time, float delay);
    public abstract void OnHide(float time, float delay);
}