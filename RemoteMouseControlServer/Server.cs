using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RemoteMouseControlServer;

public class Server : IDisposable
{

    
    private readonly UdpClient _server;
    
    private readonly List<Client> _clients = new();


    public Server(IPEndPoint ip)
    {
        _server = new UdpClient(ip);
    }

    
    public async Task ReceiveClients(CancellationToken token = default)
    {
        var clientCount = 0;
        while (clientCount < 2)
        {
            var client = await _server.ReceiveAsync(token);
            _clients.Add(new Client(client.RemoteEndPoint, Encoding.UTF8.GetString(client.Buffer)));
            clientCount++;
        }
    }
    
    public async Task Start(CancellationToken token = default)
    {
        IPEndPoint? sender = null;
        IPEndPoint? receiver = null;

        
        
        foreach (var client in _clients)
        {
            switch (client.Type)
            {
                case "Sender":
                    sender = client.RemoteEndPoint;
                    break;
                case "Receiver":
                    receiver = client.RemoteEndPoint;
                    break;
            }
        }

        var tokenReceive = await _server.ReceiveAsync(token);
        await _server.SendAsync(tokenReceive.Buffer, tokenReceive.Buffer.Length, receiver);
        var tokenVerification = await _server.ReceiveAsync(token);
        await _server.SendAsync(tokenVerification.Buffer, tokenVerification.Buffer.Length, sender);
        await Task.Run( async () =>
        {
            while (true)
            {
                var result = _server.Receive(ref sender);
                await _server.SendAsync(result, result.Length, receiver);
                Thread.Sleep(1);
            }
        }, token);
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _server.Dispose();
    }
}