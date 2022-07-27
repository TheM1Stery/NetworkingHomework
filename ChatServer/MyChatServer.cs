using System.Net;
using System.Net.Sockets;

namespace ChatServer;

public class MyChatServer : IDisposable
{
    private readonly Socket _server;
    private readonly IPAddress _ipAddress;


    public MyChatServer()
    {
        _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        _ipAddress = IPAddress.Parse("127.0.0.1");
    }
    
    
    public Task StartInfinite(CancellationToken cancellationToken = default)
    {
        // will add Task.Run() will logic later
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _server.Dispose();
        GC.SuppressFinalize(this);
    }
}