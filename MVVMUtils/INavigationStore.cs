namespace MVVMUtils;

public interface INavigationStore<TBaseViewModel>
{
    public TBaseViewModel? CurrentViewModel { get; set; }
}