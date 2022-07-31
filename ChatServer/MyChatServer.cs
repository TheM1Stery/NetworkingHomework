using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatServer;

public class MyChatServer : IDisposable
{
    private readonly Socket _server;
    private readonly EndPoint _endPoint;
    private readonly ConcurrentDictionary<int, Socket> _clients;
    private int _clientCount;

    public event Action<string>? MessageReceived;
    
    public event Action? Disconnected;
    
    public MyChatServer(string ipAddress, int port)
    {
        _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        _endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
        _clients = new ConcurrentDictionary<int, Socket>();
    }
    
    
    public async Task StartInfiniteAsync(CancellationToken cancellationToken = default)
    {
        _server.Bind(_endPoint);
        _server.Listen();
        var concurrentQueue = new ConcurrentQueue<Socket>();
        try
        {
            await Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var socket = await _server.AcceptAsync(cancellationToken);
                    concurrentQueue.Enqueue(socket);
                    _clients.TryAdd(_clientCount++, socket);
                    _ = Task.Factory.StartNew(async () =>
                    {
                        if (!concurrentQueue.TryDequeue(out var currentClient))
                        {
                            return;
                        }
                        using (currentClient)
                        {
                            var buffer = new byte[1024];
                            while (currentClient.Connected)
                            {
                                cancellationToken.ThrowIfCancellationRequested();
                                var bytes = await currentClient.ReceiveAsync(buffer, SocketFlags.None);
                                var message = Encoding.UTF8.GetString(buffer).Replace("\0", 
                                    string.Empty);
                                MessageReceived?.Invoke(message[..bytes]);
                                foreach (var (key, client) in _clients.ToList())
                                {
                                    if (client.LocalEndPoint == null)
                                        continue;
                                    try
                                    {
                                        socket.SendTo(Encoding.UTF8.GetBytes(message), SocketFlags.None, client.LocalEndPoint);
                                    }
                                    catch (Exception e) when (e is SocketException or ObjectDisposedException)
                                    {
                                        _clients.TryRemove(key, out var removedClient);
                                    }
                                }
                            }
                        }
                    }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
        catch (OperationCanceledException)
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