using UnityEngine;

public class Handset : MonoBehaviour
{
    #region Fields

    [SerializeField] GameObject light;

    #endregion

    #region Public Methods
    
    public void ToggleHandset(bool isOn)
    {
        light.SetActive(isOn);
    }

    #endregion
}
