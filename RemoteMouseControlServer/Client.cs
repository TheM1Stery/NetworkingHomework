using System.Net;

namespace RemoteMouseControlServer;

public class Client
{
    public string Type { get; }
    
    public IPEndPoint RemoteEndPoint { get; }
    
    

    public Client(IPEndPoint remoteEndPoint, string type)
    {
        RemoteEndPoint = remoteEndPoint;
        Type = type;
    }
}