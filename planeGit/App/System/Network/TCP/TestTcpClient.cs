using Cysharp.Threading.Tasks;
using System.Net;
using System.Text;
using UniRx;
using UnityEngine;

namespace MJ.Network
{
    public class TestTcpClient : MonoBehaviour
    {
        private TcpClient client;
        private void Start()
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse("192.168.0.195"), 50010);
            client = new TcpClient();
            client.Bind(endpoint).Forget();
            client.OnReceive.Subscribe(x =>
            {
                string s = Encoding.ASCII.GetString(x);
                Logger.Log(s);
            });
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                var data = DataModel.Instance.USModels;
                foreach (var index in data)
                    client?.Send(index.Serialize());
            }
        }

        private void OnApplicationQuit()
        {
            client?.Dispose();
        }
    }
}