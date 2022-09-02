using System.Net;
using System.Net.Sockets;

namespace MultiCastServer;

public class MultiCastSenderServer
{
    // private readonly object _lock = new();
    
    private TcpListener _server;

    private readonly List<Client> _clients = new();
    
    
    public MultiCastSenderServer(IPEndPoint endPoint)
    {
        _server = new TcpListener(endPoint);
    }
    
    
    public async Task Start(CancellationToken token = default)
    {
        _server.Start();
        await Task.Run( async () =>
        {
            while (true)
            {
                var client = await _server.AcceptTcpClientAsync(token);
                var clientHandler = new Client(client);
                clientHandler.SendFailed += ClientHandlerOnSendFailed;
                _clients.Add(clientHandler);
            }
        },token).ConfigureAwait(false);
    }

    private void ClientHandlerOnSendFailed(Client obj)
    {
        _clients.Remove(obj);
        obj.SendFailed -= ClientHandlerOnSendFailed;
    }

    public async Task Send<T>(T obj, CancellationToken token = default)
    {
        var list = _clients.ToList().Select(client => client.Send(obj)).ToList();
        await Task.WhenAll(list).ConfigureAwait(false);
    }
}