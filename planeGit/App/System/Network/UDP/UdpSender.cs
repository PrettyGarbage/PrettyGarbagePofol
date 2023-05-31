using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

public class UdpSender : IUdpSender
{
    #region private variables

    Subject<byte[]> subject;
    CancellationTokenSource cancellationTokenSource;
    UdpClient udpClient;

    #endregion

    #region Property

    public IObservable<byte[]> OnSend => subject;
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
    
    public static UdpSender Bind(string ip, int port)
    {
        UdpSender sender = new();
        sender.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
        return sender;
    }
    
    public void Bind(IPEndPoint endPoint)
    {
        UdpClient ??= new UdpClient();
        UdpClient.Connect(endPoint.Address, endPoint.Port);
        cancellationTokenSource ??= new CancellationTokenSource();
        subject = new Subject<byte[]>();
    }

    public void Bind(UdpClient client)
    {
        UdpClient = client;
        cancellationTokenSource ??= new CancellationTokenSource();
        subject = new Subject<byte[]>();
    }
    
    public void SendDatagram(byte[] data)
    {
        SendAsync(data).Forget();
    }

    public void SendDatagram(string data)
    {
        var bytes = Encoding.UTF8.GetBytes(data);
        SendAsync(bytes).Forget();
    }

    public virtual void Dispose()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
        subject.OnCompleted();
        subject.Dispose();
        UdpClient.Dispose();
    }
    
    async UniTaskVoid SendAsync(byte[] bytes)
    {
        var(cancelUniTask, _) = cancellationTokenSource.Token.ToUniTask();

        try
        {
            //UDP로 데이터를 송신
            var sendTask = udpClient.SendAsync(bytes, bytes.Length).AsUniTask();

            //ReceiveAsync에는 밖으로부터 캔슬하는 기능이 먹히지 않음.
            //거기서 UniTask.WhenAny와 CancellationToken.ToUniTask병용하여
            //캔슬되면 await이 반드시 종료되도록 함.
            var (hasResult, _) = await UniTask.WhenAny(sendTask, cancelUniTask);

            //캔슬되는 경우 false가 되기 때문에 그대로 종료
            if (!hasResult) return;

            //Subject에도 bytes 정보 전달
            subject.OnNext(bytes);
        }
        catch (Exception e)
        {
            Logger.LogError(e);
            subject.OnError(e);
        }
        
    }

}
