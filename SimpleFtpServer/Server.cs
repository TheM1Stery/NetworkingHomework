using System.Net;
using System.Net.Sockets;

namespace SimpleFtpServer;

public class Server
{
    private TcpListener _listener;

    public Server(IPEndPoint endPoint)
    {
        _listener = new TcpListener(endPoint);
    }

    public async Task Start(CancellationToken token = default)
    {
        _listener.Start();
        await Task.Run( async () =>
        {
            while (true)
            {
                var client = await _listener.AcceptTcpClientAsync(token);
                var task = HandleClient(client, token);
            }
        }, token);
    }



    private async Task HandleClient(TcpClient client, CancellationToken token = default)
    {
        await using var writer = new StreamWriter(client.GetStream());
        using var reader = new StreamReader(client.GetStream());
        while (client.Connected || token.IsCancellationRequested)
        {
            var path = await reader.ReadLineAsync();
            
        }
    }
}