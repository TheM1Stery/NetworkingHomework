using System.Net;
using System.Net.Sockets;
using SimpleFtpServer.Commands;

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
        await using var stream = client.GetStream();
        var streamWriter = new StreamWriter(stream){AutoFlush = true};
        var streamReader = new StreamReader(stream);
        IFtpCommand sendAllFileInfoCommand = new GetAllFileInfoCommand();
        IFtpCommand sendFileCommand = new GetFileCommand();
        IFtpCommand uploadCommand = new UploadCommand();
        while (client.Connected || token.IsCancellationRequested)
        {
            var str = await streamReader.ReadLineAsync();
            if (str == "ls")
            {
                await sendAllFileInfoCommand.HandleAsync(stream, token: token);
                continue;
            }
            if (str != null && str.Contains("upload"))
            {
                var uploadParams = str.Split(' ');
                await uploadCommand.HandleAsync(stream, uploadParams[1], token: token);
            }
            if (str == null || !str.Contains("get")) 
                continue;
            var getParameters = str.Split(' ');
            await sendFileCommand.HandleAsync(stream, getParameters[1], token);
        }
    }
}