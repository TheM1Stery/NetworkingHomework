using ChatClient.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using MVVMUtils;

namespace ChatClient.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    public INavigationStore<BaseViewModel> NavigationStore { get; }

    [ObservableProperty]
    private string? _title;

    public MainViewModel(INavigationStore<BaseViewModel> navigationStore, INavigationService<BaseViewModel> navigationService) 
        : base(navigationService)
    {
        NavigationStore = navigationStore;
        Title = "MyChatClient";
        Navigator.Navigate<LoginViewModel>();
    }
}