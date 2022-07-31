using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using MVVMUtils;

namespace ChatClient.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EnterChatCommand))]
    private string? _nickname;
    
    
    public LoginViewModel(INavigationService<BaseViewModel> navigationService) : base(navigationService)
    {
    }

    private bool CanEnter()
    {
        return !string.IsNullOrWhiteSpace(Nickname);
    }

    [RelayCommand(CanExecute = nameof(CanEnter))]
    private void EnterChat()
    {
        // Nickname never will be null because of CanEnter()
        Navigator.Navigate<ChatViewModel>();
        WeakReferenceMessenger.Default.Send(new ValueChangedMessage<string>(Nickname!)); 
    }
}