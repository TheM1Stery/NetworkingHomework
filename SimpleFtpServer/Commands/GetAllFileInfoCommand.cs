using System.Net.Sockets;
using System.Text;

namespace SimpleFtpServer.Commands;


// implementation of ls command from ftp
public class GetAllFileInfoCommand : IFtpCommand
{
    public async Task HandleAsync(NetworkStream stream,string? parameter = null, CancellationToken token = default)
    {
        var builder = new StringBuilder();
        var files = Directory.GetFiles("FtpDirectory");
        foreach (var file in files)
        {
            builder.Append(Path.GetFileName(file) + ' ');
        }
        var writer = new StreamWriter(stream) {AutoFlush = true};
        await writer.WriteLineAsync(builder.ToString());
    }
}