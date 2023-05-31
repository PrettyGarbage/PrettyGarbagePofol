using System;
using System.Net;
using System.Text;
using UniRx;
using UnityEngine;

namespace __MyAssets._02.Scripts.App.System.Network.UDP
{
    public class UdpTestNetwork : MonoBehaviour
    {
        UdpClientProcess udp;
        IPEndPoint multicastEp = new(IPAddress.Parse("227.0.1.0"), 20000);
        async void Start()
        {
            // (1) udpClient 객체 생성
            //udp = new UdpClient();

            udp = new UdpClientProcess();
            IPAddress addr = IPAddress.Parse("227.0.1.0");
            IPEndPoint end = new(addr, 20231);
            udp.Bind(end);
            udp.OnSend.Subscribe(bytes => Logger.Log(Encoding.UTF8.GetString(bytes)));

            Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ =>
            {
                OnClickEvent();
            }).AddTo(gameObject);
        }

        public void OnClickEvent()
        {
            var data = DataModel.Instance.USModels;
            foreach (var index in data)
                udp.SendDatagram(index.Serialize());
        }

        private void OnApplicationQuit()
        {
            udp.Dispose();
        }
    }
}