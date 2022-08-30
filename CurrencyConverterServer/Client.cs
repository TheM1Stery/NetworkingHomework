using System.Net.Sockets;

namespace CurrencyConverterServer;

public class Client
{
    private readonly TcpClient _client;
    private readonly CancellationToken _token;

    public Client(TcpClient client, CancellationToken token = default)
    {
        _client = client;
        _token = token;
    }

    public async Task Handle()
    {
           
    }
}