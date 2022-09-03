using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Serilog;

namespace CurrencyConverterServer;

public class Client : IDisposable
{
    private readonly TcpClient _client;
    private readonly ICurrencyDbClient _dbClient;
    private readonly ILogger _logger;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;

    public Client(TcpClient client,ICurrencyDbClient dbClient, ILogger logger)
    {
        _client = client;
        _dbClient = dbClient;
        _logger = logger;
        _writer = new StreamWriter(_client.GetStream())
        {
            AutoFlush = true
        };
        _reader = new StreamReader(_client.GetStream());
    }

    public async Task Handle(CancellationToken token = default)
    {
        while (_client.Connected || !token.IsCancellationRequested)
        {
            var json = await _reader.ReadLineAsync().ConfigureAwait(false); // getting the response from the client
            if (json is null)
                return;
            await using var memStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var request = await JsonSerializer.DeserializeAsync<ConversionRequest>(memStream, cancellationToken: token)
                .ConfigureAwait(false);
            if (request is null)
                return;
            var firstCurrency = await _dbClient.GetCurrency(request.From);
            var secondCurrency = await _dbClient.GetCurrency(request.To);
            var conversion = await _dbClient.GetCurrencyConversion(firstCurrency!, secondCurrency!);
            await _writer.WriteLineAsync(
                JsonSerializer.Serialize(new ConversionResult(conversion!.Cost * request.MoneyToConvert)));
        }
        
    }

    public void Dispose()
    {
        _client.Dispose();
        _reader.Dispose();
        _writer.Dispose();
        GC.SuppressFinalize(this);
    }
}