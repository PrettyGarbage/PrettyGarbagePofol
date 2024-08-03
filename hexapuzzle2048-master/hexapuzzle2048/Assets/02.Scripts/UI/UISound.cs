using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISound : MonoBehaviour
{
    public void PlayUISound(string key)
    {
        SoundManager.Instance.PlayUISoundInstance(key);
    }
}