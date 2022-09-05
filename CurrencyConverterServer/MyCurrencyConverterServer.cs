using System.Net;
using System.Net.Sockets;
using Serilog;

namespace CurrencyConverterServer;

public class MyCurrencyConverterServer
{
    private readonly ICurrencyDbClient _dbClient;
    private readonly ILogger _logger;
    private readonly TcpListener _server;

    private volatile int _clientCount; 
    
    public MyCurrencyConverterServer(IPEndPoint endPoint, ICurrencyDbClient dbClient,ILogger logger)
    {
        _dbClient = dbClient;
        _logger = logger;
        _server = new TcpListener(endPoint);
    }
    
    public async Task Start(CancellationToken token = default)
    {
        _server.Start();
        await Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                var tcpClient = await _server.AcceptTcpClientAsync(token);
                var writer = new StreamWriter(tcpClient.GetStream());
                var ip = tcpClient.Client.RemoteEndPoint as IPEndPoint;
                if (_clientCount > 4)
                {
                    _logger.Information("{Ip}:{Port} tried to connect, but the server is loaded", 
                        ip?.Address, ip?.Port);
                    _ = Task.Run(() =>
                    {
                        writer.WriteLine("Server is loaded");
                        writer.Dispose();
                        tcpClient.Dispose();
                    }, token);
                    continue;
                }
                var client = new Client(tcpClient, _dbClient, _logger);
                Interlocked.Increment(ref _clientCount);
                _logger.Information("{Ip}:{Port} connected to the server", ip?.Address, ip?.Port);
                var task = client.Handle(token);
                _ = task.ContinueWith(t =>
                {
                    _logger.Information("{Ip}:{Port} left the server", ip?.Address, ip?.Port);
                    client.Dispose();
                    Interlocked.Decrement(ref _clientCount);
                }, token);
            }
        }, token);
    }
}