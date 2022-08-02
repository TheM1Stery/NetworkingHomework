using System;
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

public partial class LoginViewModel : BaseViewModel
{
    private readonly IChatClient _chatClient;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EnterChatCommand))]
    private string? _nickname;


    private bool _isValid = true;
    
    private static int _counter = 0;

    private async Task ShowMessageAndExit()
    {
        Window? window = null;
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            window = desktop.MainWindow;
        }

        await MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
            {
                ContentTitle = "Server error",
                ContentMessage = "Couldn't connect to the server. Closing the program",
                ShowInCenter = true,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ButtonDefinitions = ButtonEnum.Ok,
                Icon = Icon.Error
            })
            .ShowDialog(window);
        _isValid = false;
        Environment.Exit(1);
    }
    
    public LoginViewModel(INavigationService<BaseViewModel> navigationService, IChatClient chatClient) : base(navigationService)
    {
        _chatClient = chatClient;
        if (_counter == 1)
            _chatClient.ConnectionFailed += OnChatClientOnConnectionFailed;
        _counter++;
    }

    private async void OnChatClientOnConnectionFailed()
    {
        await Dispatcher.UIThread.InvokeAsync(ShowMessageAndExit);
    }


    private bool CanEnter()
    {
        return !string.IsNullOrWhiteSpace(Nickname);
    }

    [RelayCommand(CanExecute = nameof(CanEnter))]
    private async Task EnterChat()
    {
        await _chatClient.ConnectAsync();

        if (_isValid)
        {
            _chatClient.ConnectionFailed -= OnChatClientOnConnectionFailed;
            Navigator.Navigate<ChatViewModel>();
            // Nickname never will be null because of CanEnter()
            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<string>(Nickname!)); 
        }
    }
}