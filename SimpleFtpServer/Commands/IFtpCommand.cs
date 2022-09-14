using System.Net.Sockets;

namespace SimpleFtpServer.Commands;

public interface IFtpCommand
{
    public Task HandleAsync(NetworkStream stream, string? parameter = null,CancellationToken token = default);
}