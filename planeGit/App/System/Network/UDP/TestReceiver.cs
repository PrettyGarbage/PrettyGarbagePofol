using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class TestReceiver : MonoBehaviour
{
    public IObservable<byte[]> OnReceived => subject;

    Subject<byte[]> subject;
    CancellationTokenSource cancellationTokenSource;

    UdpClient udp;
    IPEndPoint remoteEndPoint;

    private UdpReceiver receiver;
    
    void Start()
    {
        receiver = new UdpReceiver();
        IPAddress addr = IPAddress.Parse("227.0.1.0");
        IPEndPoint end = new IPEndPoint(addr, 20000);
        receiver.Bind(end, UDPType.Multicast);
        
        receiver.OnReceived.ObserveOnMainThread().Subscribe(bytes =>
        {
            Logger.Log(Encoding.UTF8.GetString(bytes) + " : " + bytes.Length);
        });
    }

    private void OnApplicationQuit()
    {
        receiver.Dispose();
    }


    public void Initialize()
    {
        //1. 객체 생성
        udp = new UdpClient();
        
        //2. UDP 로컬 IP / 포트에 바인딩
        var localEndPoint = new IPEndPoint(IPAddress.Any, 20000);
        udp.Client.Bind(localEndPoint);
        
        //3. Multicast 그룹에 조인
        IPAddress multicastIP = IPAddress.Parse("227.0.0.1");
        udp.JoinMulticastGroup(multicastIP);
        
        remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        cancellationTokenSource = new CancellationTokenSource();

        ReadAsync(cancellationTokenSource.Token).Forget();
    }
    
    async UniTaskVoid ReadAsync(CancellationToken token)
    {
        var (cancelUniTask, _) = token.ToUniTask();

        while (!token.IsCancellationRequested)
        {
            var receiveUniTask = udp.ReceiveAsync().AsUniTask();
                
            var (hasResult, result) = await UniTask.WhenAny(receiveUniTask, cancelUniTask);
                
            if (!hasResult)
                return;

            var data = Encoding.ASCII.GetString(result.Buffer, 0, result.Buffer.Length);

            Logger.Log(data);
            
            //subject.OnNext(result.Buffer);
        }
    }

    public void Test()
    {
        object input = String.Empty;
        
        Logger.Log(input is not (float or double) ? "i am not" : "i am");
    }
}