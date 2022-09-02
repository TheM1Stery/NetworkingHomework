using MediatR;

namespace MultiCastServer;

public class SocketErrorNotification : INotification
{
    public Client Client { get; set; } = null!;
}