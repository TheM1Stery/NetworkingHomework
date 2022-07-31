using System;
using ChatClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using MVVMUtils;

namespace ChatClient.ViewModels;

public partial class ChatViewModel : BaseViewModel, IRecipient<ValueChangedMessage<string>>
{
    private readonly IChatClient _chatClient;

    private string? _nickname;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendMessageCommand))]
    private string? _sentMessage;

    [ObservableProperty]
    private string? _chatText;

    private bool CanSendMessage()
    {
        return !string.IsNullOrWhiteSpace(_sentMessage);
    }
    
    public ChatViewModel(IChatClient chatClient, INavigationService<BaseViewModel> navigationService) 
        : base(navigationService)
    {
        _chatClient = chatClient;
        ChatText = string.Empty;
        _chatClient.MessageReceived += m =>
        {
            ChatText += $"{m}\n";
        };
    }


    [RelayCommand(CanExecute = nameof(CanSendMessage))]
    private void SendMessage()
    {
        _chatClient.ReceiveMessageAsync();
        // _sentMessage will never be null because of CanSendMessage()
        _chatClient.SendMessageAsync($"{_nickname}: {_sentMessage}");
    }


    public void Receive(ValueChangedMessage<string> message)
    {
       _nickname = message.Value;
    }
}