using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace ChatServer;

public class MyChatServer : IDisposable
{
    private readonly Socket _server;
    private readonly EndPoint _endPoint;
    private readonly ConcurrentDictionary<int, Socket> _clients;
    private int _clientCount;

    public event Action? Disconnected;
    
    public MyChatServer(Socket server, IPAddress address)
    {
        _server = server;
        _endPoint = new IPEndPoint(address, 27001);
        _clients = new ConcurrentDictionary<int, Socket>();
    }
    
    
    public async Task StartInfiniteAsync(CancellationToken cancellationToken = default)
    {
        _server.Bind(_endPoint);
        _server.Listen();
        var concurrentQueue = new ConcurrentQueue<Socket>();
        try
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var socket = await _server.AcceptAsync(cancellationToken);
                    concurrentQueue.Enqueue(socket);
                    _clients.TryAdd(_clientCount, socket);
                    _ = Task.Run( async () =>
                    {
                        if (!concurrentQueue.TryDequeue(out var currentClient))
                        {
                            return;
                        }
                        using (currentClient)
                        {
                            var buffer = new byte[1024];
                            while (true)
                            {
                                cancellationToken.ThrowIfCancellationRequested();
                                await currentClient!.ReceiveAsync(buffer, SocketFlags.None);
                                foreach (var (key, client) in _clients.ToList())
                                {
                                    if (client.LocalEndPoint == null)
                                        continue;
                                    try
                                    {
                                        socket.SendTo(buffer, SocketFlags.None, client.LocalEndPoint);
                                    }
                                    catch (Exception)
                                    {
                                        _clients.Remove(key, out currentClient);
                                    }
                                }
                            }
                        }
                        
                    }, cancellationToken);
                }
            }, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception)
        {
            Disconnected?.Invoke();
        }
        
    }

    public void Dispose()
    {
        _server.Dispose();
        GC.SuppressFinalize(this);
    }
}