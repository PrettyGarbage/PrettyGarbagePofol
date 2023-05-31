using Cysharp.Threading.Tasks;
using System.Net;

public interface ITcpServer
{
    public void Bind(IPEndPoint endPoint);

    public void Dispose();
}