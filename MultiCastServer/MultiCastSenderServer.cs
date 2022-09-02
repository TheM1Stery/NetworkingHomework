using System.Net;
using System.Net.Sockets;
using MediatR;

namespace MultiCastServer;

public class MultiCastSenderServer : INotificationHandler<SocketErrorNotification>
{
    private readonly IMediator _mediator;
    // private readonly object _lock = new();
    
    private TcpListener _server;

    private readonly List<Client> _clients = new();
    
    
    public MultiCastSenderServer(IMediator mediator,IPEndPoint endPoint)
    {
        _mediator = mediator;
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
                var clientHandler = new Client(_mediator,client);
                _clients.Add(clientHandler);
            }
        },token).ConfigureAwait(false);
    }
    
    public async Task Send<T>(T obj, CancellationToken token = default)
    {
        var list = _clients.Select(client => client.Send(obj)).ToList();
        await Task.WhenAll(list).ConfigureAwait(false);
    }

    public Task Handle(SocketErrorNotification notification, CancellationToken cancellationToken)
    {
        _clients.Remove(notification.Client);
        return Task.CompletedTask;
    }
}