using System.Net;
using System.Net.Sockets;

public enum ConnectionType
{
    Broadcast,
    Multicast,
}

public interface IUdpClientProcess
{
    public void Bind(IPEndPoint endpoint, ConnectionType type = ConnectionType.Broadcast);
    public void Dispose();
}