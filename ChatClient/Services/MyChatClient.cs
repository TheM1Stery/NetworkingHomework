using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatClient.Services;

public class MyChatClient : IChatClient
{
    private readonly Socket _client;

    private readonly EndPoint _serverEndPoint;

    public event Action<string>? MessageReceived;

    public event Action? Disconnected;
    
    
    public MyChatClient(string address, int port, bool isDns)
    {
        _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        if (isDns == false)
        {
            _serverEndPoint = new IPEndPoint(IPAddress.Parse(address), port);
            return;
        }

        _serverEndPoint = new DnsEndPoint(address, port);
    }

    public async Task ReceiveMessageAsync(CancellationToken token = default)
    {
        if (!_client.Connected)
        {
            await _client.ConnectAsync(_serverEndPoint, token);
        }
        try
        {
            await Task.Run( async () =>
            {
                var buffer = new byte[1024];
                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    await _client.ReceiveAsync(buffer, SocketFlags.None);
                    var message = Encoding.UTF8.GetString(buffer);
                    MessageReceived?.Invoke(message);
                    Array.Clear(buffer);
                }
            }, token);
        }
        catch (OperationCanceledException)
        {
            Disconnected?.Invoke();
        }
        
    }
    
    public async Task SendMessageAsync(string message)
    {
        if (!_client.Connected)
        {
            await _client.ConnectAsync(_serverEndPoint);
        }
        await _client.SendAsync(Encoding.UTF8.GetBytes(message), SocketFlags.None);
    }
}