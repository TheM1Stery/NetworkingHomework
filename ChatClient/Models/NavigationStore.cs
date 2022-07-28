using ChatClient.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using MVVMUtils;

namespace ChatClient.Models;

public partial class NavigationStore : ObservableObject, INavigationStore<BaseViewModel>
{
    [ObservableProperty]
    private BaseViewModel? _currentViewModel;
}