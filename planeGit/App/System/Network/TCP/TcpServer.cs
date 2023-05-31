using Cysharp.Threading.Tasks;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UniRx;
using UnityEngine;

public class TcpServer : ITcpServer
{
    #region private variables

    private const int MaxBufferSize = 4096;
    private readonly Subject<byte[]> _receiveDataSubject = new();
    private CancellationTokenSource _cancellationTokenSource = new();
    private TcpListener _listener = null;
    private TcpClient _client;
    private NetworkStream _stream;
    private bool _isActive = false;
    private byte[] _buffer = new byte[MaxBufferSize];

    #endregion

    #region Properties
    
    public IObservable<byte[]> ReceiveDataObservable => _receiveDataSubject;
    public byte[] Buffer => _buffer;
    public TcpClient Client => _client;

    #endregion

    #region Public Methods

    public static TcpServer Bind(string ip, int port)
    {
        var server = new TcpServer();
        server.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
        return server;
    }
    
    public void Bind(IPEndPoint endPoint)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        
        //_receiveDataSubject.Subscribe(x => {})

        StartServer(endPoint).Forget();
    }


    public void Dispose()
    {
        _isActive = false;
        _receiveDataSubject?.Dispose();
        _cancellationTokenSource?.Dispose();
        _listener?.Server?.Dispose();
        _client?.Dispose();
        _stream?.Dispose();
    }

    #endregion

    #region Private methods

    private async UniTask StartServer(IPEndPoint endPoint)
    {
        _listener = new(endPoint);
        _listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        _listener.Start();
        _isActive = true;
        
        byte[] buffer = new byte[MaxBufferSize];

        //예외상황이 발생하면 MainThread로 돌려준다.
        await using (UniTask.ReturnToMainThread())
        {
            try
            {
                await UniTask.SwitchToThreadPool();
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    _client = await _listener.AcceptTcpClientAsync();

                    Logger.Log("### AcceptTcpClientAsync");
                    _stream = _client.GetStream();
                    int i;
                    while ((i = await _stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                    {
                        var data = new byte[i];
                        Array.Copy(buffer, data, i);
                        _receiveDataSubject.OnNext(data);
                        _buffer = data;
                    }

                    _client.Close();
                    _client.Dispose();
                    await _stream.DisposeAsync();
                }
            }
            catch (Exception e)
            {
                Logger.Log(e.Message);
                _isActive = false;
                _listener?.Server.Dispose();
            }
            
        }
    }
    
    
    

    #endregion
}