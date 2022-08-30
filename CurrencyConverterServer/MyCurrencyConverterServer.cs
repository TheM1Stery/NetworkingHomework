using System.Net;
using System.Net.Sockets;

namespace CurrencyConverterServer;

public class MyCurrencyConverterServer
{
    private TcpListener _server;
    
    public MyCurrencyConverterServer(string ip, int port)
    {
        _server = new TcpListener(IPAddress.Parse(ip), port);
    }
    
    public async Task Start(CancellationToken token = default)
    {
        _server.Start();
        await Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                var tcpClient = await _server.AcceptTcpClientAsync(token);
                var client = new Client(tcpClient, token);
                var task = client.Handle();
                _ = task.ContinueWith(t =>
                {
                    
                }, token);
            }
        }, token);
    }
}