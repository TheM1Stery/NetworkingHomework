namespace MVVMUtils;

public interface IViewModelFactory<TBaseViewModel>
{
    public TBaseViewModel Create<TViewModel>() where TViewModel : TBaseViewModel;
}