using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MicComponent : MonoBehaviour
{
    #region Variables
    
    private AudioSource _audioSource;
    private CancellationTokenSource _cancellationTokenSource;
    private readonly Subject<byte[]> _sendPacketSubject = new();
    private readonly Subject<bool> _isSpeakingSubject = new();
    private string _micName = string.Empty;

    #endregion

    #region Property

    public IObservable<byte[]> OnSendPacket => _sendPacketSubject;

    #endregion
    
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        
        MicrophoneCaptureStart();
    }
    
    private void OnDestroy()
    {
        _cancellationTokenSource?.Cancel();
        Microphone.End(CheckDevices());
        _sendPacketSubject.Dispose();
    }

    #region Private Methods

    /// <summary>
    /// 마이크 장치 확인
    /// </summary>
    /// <returns>deviceName</returns>
    private string CheckDevices()
    {
        var deviceName = 
            Microphone.devices
                .FirstOrDefault(device => device.ToLower().Contains("vive"));

        if (deviceName.IsNullOrEmpty() && Microphone.devices.Length > 0)
            deviceName = Microphone.devices.FirstOrDefault();

        var comment = deviceName.IsNullOrEmpty()? "Microphone Not Found" : "is Selected";
        
        Logger.Log(deviceName + comment, Logger.LogLevel.Resource);
        return deviceName;
    }
    
    /// <summary>
    /// 마이크 장치 입력 이벤트 처리
    /// </summary>
    private void MicrophoneCaptureStart()
    {
        _micName = CheckDevices();

        if (_micName.IsNullOrEmpty())
        {
            Logger.Log("마이크 장치가 감지 되지 않습니다.", Logger.LogLevel.Error);
            return;
        }
        
        _audioSource.clip = Microphone.Start(_micName
            , true, 1, 8000);

        _cancellationTokenSource = new CancellationTokenSource();

        VoiceCapturing(_micName).Forget();
    }

    private async UniTaskVoid VoiceCapturing(string deviceName)
    {
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            if (Microphone.GetPosition(deviceName) <= 0)
                continue;

            var data = GetClipData(_audioSource.clip);
            
            if(GetAverageVolume(_audioSource.clip) > 0.5f)
                _isSpeakingSubject.OnNext(true);
            
            _sendPacketSubject.OnNext(data);

            await UniTask.Delay(1000, cancellationToken: _cancellationTokenSource.Token);
            
            _isSpeakingSubject.OnNext(false);
        }

    }

    private byte[] GetClipData(AudioClip clip)
    {
        var floatData = new float[clip.samples * clip.channels];
        clip.GetData(floatData, 0);
        
        var byteData = new byte[floatData.Length * 4];
        Buffer.BlockCopy(floatData, 0, byteData, 0, byteData.Length);

        return byteData;
    }
    
    private float GetAverageVolume(AudioClip clip)
    {
        var floatData = new float[clip.samples * clip.channels];
        clip.GetData(floatData, 0);
        
        var sum = 0f;
        foreach (var s in floatData)
        {
            sum += Mathf.Abs(s);
        }

        return sum / floatData.Length;
    }


    #endregion
}