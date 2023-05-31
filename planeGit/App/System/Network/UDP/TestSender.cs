using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UniRx;
using UnityEngine;

public class TestSender : MonoBehaviour
{
    UdpClient udp;
    IPEndPoint multicastEp = new(IPAddress.Parse("227.0.1.0"), 20000);

    UdpSender sender;
    async void Start()
    {
        // (1) udpClient 객체 생성
        //udp = new UdpClient();

        sender = new UdpSender();
        IPAddress addr = IPAddress.Parse("227.0.1.0");
        IPEndPoint end = new(addr, 20231);
        sender.Bind(end);
        sender.OnSend.Subscribe(bytes => Logger.Log(Encoding.UTF8.GetString(bytes)));

        Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ =>
        {
            OnClickEvent();
        }).AddTo(gameObject);
    }

    public void OnClickEvent()
    {
        var data = DataModel.Instance.USModels;
        foreach (var index in data)
            sender.SendDatagram(index.Serialize());
    }

    private void OnApplicationQuit()
    {
        sender.Dispose();
    }
}