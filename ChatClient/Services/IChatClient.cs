using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChatClient.Services;

public interface IChatClient
{
    public event Action<string>? MessageReceived;

    public event Action? Disconnected;

    public event Action? ConnectionFailed;
    
    
    /// <summary>
    /// will run endlessly till the application finishes
    /// </summary>
    /// <returns>awaitable task</returns>
    public Task ReceiveMessageAsync(CancellationToken token = default);

    public Task SendMessageAsync(string message);
    
    public Task ConnectAsync();
}