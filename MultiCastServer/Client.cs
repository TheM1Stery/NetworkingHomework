using System.Net.Sockets;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace MultiCastServer;

public class Client : IDisposable
{
    private readonly TcpClient _client;

    private readonly StreamWriter _writer;

    private readonly StreamReader _reader;


    public event Action<Client>? SendFailed;
    
    public Client(TcpClient client)
    {
        _client = client;
        _writer = new StreamWriter(_client.GetStream())
        {
            AutoFlush = true
        };
        _reader = new StreamReader(_client.GetStream());
    }
    
    public async Task Send<T>(T obj)
    {
        try
        {
            await _writer.WriteLineAsync(JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }));
        }
        catch (Exception)
        {
            SendFailed?.Invoke(this);
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _client.Dispose();
        _writer.Dispose();
        _reader.Dispose();
    }
}