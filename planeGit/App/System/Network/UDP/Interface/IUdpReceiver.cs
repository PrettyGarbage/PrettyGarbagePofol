using System.Net;
public enum UDPType
{
    Broadcast,
    Multicast
}
public interface IUdpReceiver
{
    public void Bind(IPEndPoint endpoint, UDPType type = UDPType.Broadcast );

    public void Dispose();
}