using Cysharp.Threading.Tasks;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UniRx;

namespace MJ.Network
{
    public class TcpClient : ITcpClient
    {
        #region variables

        private const int MaxSize = 4096;
        private System.Net.Sockets.TcpClient tcpClient;
        private NetworkStream stream;
        private byte[] buffer = new byte[MaxSize];
        private readonly Subject<byte[]> subject = new();
        private bool _isConnected = false;

        #endregion

        #region Property

        public IObservable<byte[]> OnReceive => subject;
        public byte[] Buffer => buffer;
        public bool IsConnected => _isConnected;

        #endregion

        #region public methods

        public void Dispose()
        {
            _isConnected = false;
            stream?.Dispose();
            subject?.Dispose();
            tcpClient?.Dispose();
        }
        
        public static async UniTask<TcpClient> Bind(string ip, int port)
        {
            var client = new TcpClient();
            await client.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            return client;
        }

        public async UniTask Bind(IPEndPoint endPoint)
        {
            await Connect(endPoint);
        }

        public void Send(byte[] bytes)
        {
            SendAsync(bytes).Forget();
        }

        public void Send(string message)
        {
            var bytes = Encoding.ASCII.GetBytes(message);
            SendAsync(bytes).Forget();
        }

        #endregion

        #region private methods

        private async UniTask Connect(IPEndPoint endpoint)
        {
            tcpClient = new System.Net.Sockets.TcpClient();
            tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            
            await tcpClient.ConnectAsync(endpoint.Address, endpoint.Port);
            stream = tcpClient.GetStream();
            _isConnected = true;
            
            if(stream.CanRead)
                stream.BeginRead(buffer, 0, buffer.Length, OnRead, null);
        }
        
        private async UniTaskVoid SendAsync(byte[] bytes)
        {
            try
            {
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
            catch (Exception e)
            {
                Logger.Log("Error while send message: " + e.Message);
            }
        }

        private void OnRead(IAsyncResult result)
        {
            try
            {
                var bytesRead = stream.EndRead(result);
                
                if (bytesRead <= 0)
                    return;
                
                subject.OnNext(buffer);
                buffer = new byte[MaxSize];
                stream.BeginRead(buffer, 0, MaxSize, OnRead, null);
            }
            catch (Exception e)
            {
                Logger.Log("Error while reading from stream: " + e.Message);
                subject.OnError(e);
            }
        }

        #endregion
    }
}