using UnityEngine;

using TMPro;

public class DisplayVersion : MonoBehaviour
{
    [SerializeField] TMP_Text _text;

    void Start()
    {
        if (_text == null)
        {
            _text = GetComponent<TMP_Text>();
        }

        if (_text != null)
        {
            _text.text = BuildManager.Instance.GetVersion();

            if (BuildManager.Instance.GetBuildPhase() == BUILD_PHASE.DEV)
            {
                _text.text += "_" + BuildManager.Instance.GetBuildPhase();
            }
        }
    }
}