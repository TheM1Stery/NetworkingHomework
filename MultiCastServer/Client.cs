using System.Net.Sockets;
using System.Text.Encodings.Web;
using System.Text.Json;
using MediatR;

namespace MultiCastServer;

public class Client
{
    private readonly IMediator _mediator;
    private readonly TcpClient _client;

    private StreamWriter _writer;

    private StreamReader _reader;

    
    public Client(IMediator mediator,TcpClient client)
    {
        _mediator = mediator;
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
            await _writer.WriteLineAsync(JsonSerializer.Serialize(obj, new JsonSerializerOptions()
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }));
        }
        catch (Exception)
        {
            await _mediator.Publish(new SocketErrorNotification()
            {
                Client = this
            });
        }
    }
}