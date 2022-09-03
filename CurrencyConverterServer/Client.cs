using System.Net.Sockets;
using Serilog;

namespace CurrencyConverterServer;

public class Client : IDisposable
{
    private readonly TcpClient _client;
    private readonly ILogger _logger;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;

    public Client(TcpClient client, ILogger logger)
    {
        _client = client;
        _logger = logger;
        _writer = new StreamWriter(_client.GetStream());
        _reader = new StreamReader(_client.GetStream());
    }

    public async Task Handle(CancellationToken token = default)
    {
        var json = await _reader.ReadLineAsync(); // getting the response from the client
        
    }

    public void Dispose()
    {
        _client.Dispose();
        _reader.Dispose();
        _writer.Dispose();
    }
}