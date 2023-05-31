using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

public class UdpReceiver : IUdpReceiver
{
    #region private variables

    Subject<byte[]> subject;
    CancellationTokenSource cancellationTokenSource;
    UdpClient udpClient;

    #endregion

    #region Properties
    public IObservable<byte[]> OnReceived => subject;
    public float Timeout { get; set; } = 15f;
    
    public UdpClient UdpClient
    {
        get => udpClient;
        set
        {
            udpClient?.Close();
            udpClient = value;
        }
    }

    #endregion

    #region public methods

    public static UdpReceiver Bind(string ip, int port, UDPType type)
    {
        UdpReceiver receiver = new();
        receiver.Bind(new IPEndPoint(IPAddress.Parse(ip), port), type);
        return receiver;
    }

    public void Bind(IPEndPoint endpoint, UDPType type)
    {
        udpClient ??= new UdpClient();
        udpClient.ExclusiveAddressUse = false;
        udpClient.Client
            .SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

        var localEndPoint = new IPEndPoint(IPAddress.Any, endpoint.Port);
        udpClient.Client.Bind(localEndPoint);
        
        if (type is UDPType.Multicast)
            udpClient.JoinMulticastGroup(endpoint.Address);

        subject = new Subject<byte[]>();
        cancellationTokenSource = new CancellationTokenSource();
        ReadAsync(cancellationTokenSource.Token).Forget();
    }

    public void Dispose()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
        subject.OnCompleted();
        subject.Dispose();
        udpClient.Dispose();
    }

    #endregion

    #region private methods

    async UniTaskVoid ReadAsync(CancellationToken token)
    {
        var (cancelUniTask, _) = token.ToUniTask();

        while (!token.IsCancellationRequested)
        {
            try
            {
                var receiveUniTask = udpClient.ReceiveAsync().AsUniTask();

                var (hasResult, result) = await UniTask.WhenAny(receiveUniTask, cancelUniTask);
                
                if (!hasResult)
                    return;

                subject.OnNext(result.Buffer);
            }
            catch (Exception e)
            {
                subject.OnError(e);
            }
        }
    }

    #endregion
}
