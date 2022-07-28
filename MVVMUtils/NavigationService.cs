namespace MVVMUtils;


public class NavigationService<TBaseViewModel> : INavigationService<TBaseViewModel>
{
    private readonly INavigationStore<TBaseViewModel> _store;
    private readonly IViewModelFactory<TBaseViewModel> _factory;

    public NavigationService(INavigationStore<TBaseViewModel> store, IViewModelFactory<TBaseViewModel> factory)
    {
        _store = store;
        _factory = factory;
    }

    public void Navigate<TViewModel>() where TViewModel : TBaseViewModel
    {
        _store.CurrentViewModel = _factory.Create<TViewModel>();
    }
}