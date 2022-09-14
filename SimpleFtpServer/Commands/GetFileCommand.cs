using System.Net.Sockets;

namespace SimpleFtpServer.Commands;

public class GetFileCommand : IFtpCommand
{
    public async Task HandleAsync(NetworkStream stream, string? parameter, CancellationToken token = default)
    {
        if (parameter is null)
        {
            throw new ArgumentException("parameter was null");
        }
        
        var writer = new BinaryWriter(stream);
        var bytes = await File.ReadAllBytesAsync($"FtpDirectory/{parameter}", token);
        writer.Write(bytes.Length);
        writer.Flush();
        writer.Write(bytes);
        writer.Flush();
    }
}