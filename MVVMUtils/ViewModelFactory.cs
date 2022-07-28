namespace MVVMUtils;

public class ViewModelFactory<TBaseViewModel> : IViewModelFactory<TBaseViewModel>
{
    private readonly Dictionary<Type, Func<TBaseViewModel>> _factories;

    public ViewModelFactory(Dictionary<Type, Func<TBaseViewModel>> factories)
    {
        _factories = factories;
    }
    
    
    public TBaseViewModel Create<TViewModel>() where TViewModel : TBaseViewModel
    {
        if (!_factories.TryGetValue(typeof(TViewModel), out var factory))
        {
            throw new ArgumentException("Invalid ViewModel");
        }

        return factory();
    }
}