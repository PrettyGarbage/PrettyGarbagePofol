using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class VoiceInfo
{
    [SerializeField]
    private string _key;
    [SerializeField]
    private AudioClip[] audioClips;

    public string Key { get { return _key; } }
    public AudioClip[] AudioClips { get { return audioClips; } }
}

#if UNITY_EDITOR
[CreateAssetMenu(fileName = "VoiceData.asset", menuName = "gbros/VoiceData", order = 1)]
#endif
public class VoiceData : ScriptableObject{

	[SerializeField]
	private List<VoiceInfo> _voiceInfoList;

    public List<VoiceInfo> VoiceInfoList { get { return _voiceInfoList; } }

}
 