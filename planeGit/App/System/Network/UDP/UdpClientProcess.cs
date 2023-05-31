
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UniRx;

public class UdpClientProcess : IUdpClientProcess
{
    #region variables

    private UdpClient _client;
    private Subject<byte[]> _receiveSubject;
    private Subject<byte[]> _sendSubject;
    private CancellationTokenSource _cancellationTokenSource;
    private IPEndPoint _localEndPoint;
    private ConnectionType _connectionType = ConnectionType.Broadcast;

    #endregion

    #region Properties

    public IObservable<byte[]> OnSend => _sendSubject;
    public IObservable<byte[]> OnReceive => _receiveSubject;

    #endregion

    #region public methods

    public void SendDatagram(byte[] data)
    {
        SendAsync(data, null).Forget();
    }

    public void SendDatagram(string data)
    {
        var bytes = Encoding.ASCII.GetBytes(data);
        SendAsync(bytes, null).Forget();
    }
    
    public void SendDatagram(byte[] data, IPEndPoint endPoint)
    {
        SendAsync(data, endPoint).Forget();
    }
    
    public void SendDatagram(string data, IPEndPoint endPoint)
    {
        var bytes = Encoding.ASCII.GetBytes(data);
        SendAsync(bytes, endPoint).Forget();
    }
    
    public void Bind(IPEndPoint endpoint, ConnectionType type = ConnectionType.Broadcast)
    {
        _connectionType = type;
        _client ??= new UdpClient();
        _client.ExclusiveAddressUse = false;
        _client.Client
            .SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        
        var localEndpoint = new IPEndPoint(IPAddress.Any, endpoint.Port);
        _client.Client.Bind(localEndpoint);

        if (type is ConnectionType.Multicast)
            _client.JoinMulticastGroup(endpoint.Address);

        _receiveSubject = new Subject<byte[]>();
        _sendSubject = new Subject<byte[]>();
        _cancellationTokenSource = new CancellationTokenSource();
        _localEndPoint = endpoint;
        
        ReadAsync(_cancellationTokenSource.Token).Forget();
    }

    public void Dispose()
    {
        if(_connectionType is ConnectionType.Multicast)
            _client.DropMulticastGroup(_localEndPoint.Address);
        
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _receiveSubject.OnCompleted();
        _receiveSubject.Dispose();
        _client.Dispose();
    }
    
    #endregion

    #region Private Methods

    /// <summary>
    /// 비동기 데이터 수신
    /// </summary>
    /// <param name="token">캔슬레이션 토큰</param>
    private async UniTaskVoid ReadAsync(CancellationToken token)
    {
        var (cancelUniTask, _) = token.ToUniTask();

        while (!token.IsCancellationRequested)
        {
            try
            {
                var receiveUniTask = _client.ReceiveAsync().AsUniTask();

                var (hasResult, result) = await UniTask.WhenAny(receiveUniTask, cancelUniTask);
                
                if (!hasResult)
                    return;

                _receiveSubject.OnNext(result.Buffer);
            }
            catch (Exception e)
            {
                _receiveSubject.OnError(e);
            }
        }
    }

    private async UniTaskVoid SendAsync(byte[] bytes, [CanBeNull]IPEndPoint endPoint)
    {
        var(cancelUniTask, _) = _cancellationTokenSource.Token.ToUniTask();

        try
        {
            //UDP로 데이터를 송신
            var sendTask = _client.SendAsync(bytes, bytes.Length, 
                endPoint ?? _localEndPoint).AsUniTask();

            //ReceiveAsync에는 밖으로부터 캔슬하는 기능이 먹히지 않음.
            //거기서 UniTask.WhenAny와 CancellationToken.ToUniTask병용하여
            //캔슬되면 await이 반드시 종료되도록 함.
            var (hasResult, _) = await UniTask.WhenAny(sendTask, cancelUniTask);

            //캔슬되는 경우 false가 되기 때문에 그대로 종료
            if (!hasResult) return;

            //Subject에도 bytes 정보 전달
            _sendSubject.OnNext(bytes);
        }
        catch (Exception e)
        {
            Logger.LogError(e);
            _sendSubject.OnError(e);
        }
    }

    #endregion
}
