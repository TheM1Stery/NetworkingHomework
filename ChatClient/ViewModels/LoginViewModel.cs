using System.Threading.Tasks;
using Avalonia.Controls;
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
    
    
    public LoginViewModel(INavigationService<BaseViewModel> navigationService, IChatClient chatClient) : base(navigationService)
    {
        _chatClient = chatClient;
        _chatClient.ConnectionFailed += () =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
                {
                    ContentTitle = "Server error",
                    ContentMessage = "Couldn't connect to the database",
                    ShowInCenter = true,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    ButtonDefinitions = ButtonEnum.Ok,
                    Icon = Icon.Error
                }).Show();
                _isValid = false;
            });
        };
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
            Navigator.Navigate<ChatViewModel>();
            // Nickname never will be null because of CanEnter()
            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<string>(Nickname!)); 
        }
    }
}