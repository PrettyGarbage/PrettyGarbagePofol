using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Text;

[System.Serializable]
public class  AudioDataInfo
{
	[SerializeField]
	private string key;
	[SerializeField]
	private AudioClipInfo audioClipInfo;

	public AudioDataInfo(string key, AudioClipInfo audioClipInfo){
		this.key = key;
		this.audioClipInfo = audioClipInfo;
	}

    public string Key
    {
        get
        {
            return key;
        }
    }

    public AudioClipInfo AudioClipInfo
    {
        get
        {
            return audioClipInfo;
		}
    }
}

#if UNINY_EDITOR
[CreateAssetMenu(fileName = "AudioDataBase.asset", menuName = "gbros/AudioDataBase", order = 1)]
#endif
[Serializable]
public class AudioDataBase : ScriptableObject{

	public const string DataPath = "Assets/07.Data/AudioDataBase.asset";
	
	[SerializeField]
	private List<AudioDataInfo> _audioDataInfoList = new List<AudioDataInfo>();
	//private Dictionary<string, AudioClipInfo>  _audioDataBaseDic;

	public int Count { get { return _audioDataInfoList.Count;}}
	
    public List<AudioDataInfo> AudioDataInfoList
    {
        get
        {
            return _audioDataInfoList;
        }
    }

    public bool AddData(string key, AudioClipInfo audioClipInfo){
		if(GetAudioClipInfo(key)!=null){
			return false;
		}else {
			_audioDataInfoList.Add(new AudioDataInfo(key, audioClipInfo));
			return true;
		}
		
	}

	public void RemoveData(string key){
		Debug.Log("RemoveData ket : " + key);
		for (int i = 0; i < _audioDataInfoList.Count; i++)
		{
			if(_audioDataInfoList[i].Key.Equals(key)){
				_audioDataInfoList.RemoveAt(i);
				break;
			}
		}
	}
	
	public AudioClipInfo GetAudioClipInfo(string key){
		for (int i = 0; i < _audioDataInfoList.Count; i++)
		{
			if(_audioDataInfoList[i].Key.Equals(key)){
				return _audioDataInfoList[i].AudioClipInfo;
			}
		}
		return null;
	}

    public void CreateEnumFile()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("public enum AudioDataKey {");
        for (int i = 0; i < _audioDataInfoList.Count; i++)
        {
            sb.AppendLine("     " + _audioDataInfoList[i].Key + " = " + i);
            if (i < _audioDataInfoList.Count - 1)
            {
                sb.Append(",");
            }
        }
        sb.Append("}");

        File.WriteAllText("Assets/02.Scripts/Common/Sound/AudioDataKey.cs", sb.ToString());
    }

#if UNITY_EDITOR
    public static AudioDataBase CreateAssest()
    {
		AudioDataBase audioDataBase;
		if(File.Exists(DataPath)){
			Debug.Log("Exists");
			audioDataBase = (AudioDataBase)AssetDatabase.LoadAssetAtPath(DataPath, typeof(AudioDataBase));
		}
		else {
			audioDataBase = CreateInstance<AudioDataBase>();
			AssetDatabase.CreateAsset(audioDataBase, DataPath);
			AssetDatabase.Refresh();
		}
		
		
		return audioDataBase;
    }
#endif
}