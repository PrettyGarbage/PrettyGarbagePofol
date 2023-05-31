using System.Net;
using System.Net.Sockets;
using System.Text;
using UniRx;
using UnityEngine;

public class TestTcpListener : MonoBehaviour
{
    private TcpServer server;

    private void Start()
    {
        server = new TcpServer();
        IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(GetLocalIPAddress()), 50001);
        server.Bind(endpoint);
            
        server.ReceiveDataObservable.Subscribe(data =>
        {
            Logger.Log("Received: " + data.Length);
            string s = Encoding.ASCII.GetString(data);
            Logger.Log(s);
        });
        Logger.Log("Tcp Listener Started");
    }

    private void ApplicationQuit()
    {
        server.Dispose();
        
    }
    
    private string GetLocalIPAddress()
    {
        IPHostEntry host;
        string localIP = "";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }
        return localIP;
    }
}