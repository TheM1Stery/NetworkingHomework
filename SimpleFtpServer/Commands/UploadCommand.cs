using System.Net.Sockets;

namespace SimpleFtpServer.Commands;

public class UploadCommand : IFtpCommand
{
    public async Task HandleAsync(NetworkStream stream, string? parameter = null, CancellationToken token = default)
    {
        // read byte count, read bytes, write to file
        var binaryReader = new BinaryReader(stream);
        var count = binaryReader.ReadInt32();
        await using var fileStream = new FileStream($"FtpDirectory/{parameter}", FileMode.OpenOrCreate, FileAccess.Write);
        for (var i = 0; i < count; i++)
        {
            fileStream.WriteByte(binaryReader.ReadByte());
        }
    }
}