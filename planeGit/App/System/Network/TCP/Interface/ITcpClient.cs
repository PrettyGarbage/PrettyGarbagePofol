using Cysharp.Threading.Tasks;
using System.Net;

namespace MJ.Network
{
    public interface ITcpClient
    {
        public void Dispose();
        public UniTask Bind(IPEndPoint endPoint);
        public void Send(string message);
    }   
}