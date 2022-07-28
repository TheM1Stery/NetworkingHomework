using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using MVVMUtils;

namespace ChatClient.ViewModels;

public partial class ChatViewModel : BaseViewModel, IRecipient<ValueChangedMessage<string>>
{

    [ObservableProperty]
    private string? _nickname;
    
    public ChatViewModel(INavigationService<BaseViewModel> navigationService) : base(navigationService)
    {
    }


    public void Receive(ValueChangedMessage<string> message)
    {
        Nickname = message.Value;
    }
}