using System.Net;

public interface IUdpSender
{
    public void Bind(IPEndPoint endPoint);

    public void Dispose();
}