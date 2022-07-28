using CommunityToolkit.Mvvm.ComponentModel;
using MVVMUtils;

namespace ChatClient.ViewModels;

public abstract class BaseViewModel : ObservableObject
{
    protected INavigationService<BaseViewModel> Navigator;

    protected BaseViewModel(INavigationService<BaseViewModel> navigationService)
    {
        Navigator = navigationService;
    }
}