using System.Net;
using System.Net.Sockets;
using Serilog;

namespace CurrencyConverterServer;

public class MyCurrencyConverterServer
{
    private readonly ICurrencyDbClient _dbClient;
    private readonly ILogger _logger;
    private TcpListener _server;
    
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
                var client = new Client(tcpClient, _dbClient, _logger);
                var ip = tcpClient.Client.RemoteEndPoint as IPEndPoint;
                _logger.Information("{ip} connected to the server", ip?.Address);
                var task = client.Handle(token);
                _ = task.ContinueWith(t =>
                {
                    _logger.Information("{ip} left the server", ip?.Address);
                    client.Dispose();
                }, token);
            }
        }, token);
    }
}