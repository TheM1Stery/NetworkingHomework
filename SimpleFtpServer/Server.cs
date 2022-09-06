using System.Net;
using System.Net.Sockets;

namespace SimpleFtpServer;

public class Server
{
    private readonly string _filesPath;
    private readonly TcpListener _listener;

    public Server(IPEndPoint endPoint, string filesPath)
    {
        _filesPath = filesPath;
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
        var stream = client.GetStream();
        while (client.Connected || token.IsCancellationRequested)
        {
            
        }
    }
}