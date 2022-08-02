using System;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using ChatClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using MVVMUtils;

namespace ChatClient.ViewModels;

public partial class ChatViewModel : BaseViewModel, IRecipient<ValueChangedMessage<string>>
{
    private readonly IChatClient _chatClient;

    private string? _nickname;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendMessageCommand))]
    private string? _sentMessage;

    // [ObservableProperty]
    // private string? _chatText;

    private readonly StringBuilder _chatText = new();


    public string ChatText => _chatText.ToString();


    private bool CanSendMessage()
    {
        return !string.IsNullOrWhiteSpace(_sentMessage);
    }


    private static async Task ShowMessageAndExit()
    {
        Window? window = null;
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            window = desktop.MainWindow;
        }
        await MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
        {
            ContentTitle = "Server error",
            ContentMessage = "Couldn't connect to the server. Please contact the admin to restart the server or " +
                             "check your internet connection. The program will be closed",
            ShowInCenter = true,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            ButtonDefinitions = ButtonEnum.Ok,
            Icon = Icon.Error
        }).ShowDialog(window);
        Environment.Exit(1);
    }

    private static int _counter = 0;
    
    public ChatViewModel(IChatClient chatClient, INavigationService<BaseViewModel> navigationService)
        : base(navigationService)
    {
        WeakReferenceMessenger.Default.Register(this);
        _chatClient = chatClient;
        if (_counter == 2)
        {
            _chatClient.MessageReceived += m =>
            {
                _chatText.Append($"{m}\n");
                OnPropertyChanged(nameof(ChatText));
            };
            _chatClient.Disconnected += () =>
            {
                Dispatcher.UIThread.InvokeAsync(ShowMessageAndExit);
            };
        }
        _counter++;
    }

    [RelayCommand(CanExecute = nameof(CanSendMessage))]
    private void SendMessage()
    {
        _chatClient.SendMessageAsync($"{_nickname}: {_sentMessage}");
        SentMessage = string.Empty;
    }


    public void Receive(ValueChangedMessage<string> message)
    {
       _nickname = message.Value;
       _chatClient.ReceiveMessageAsync();
    }
}