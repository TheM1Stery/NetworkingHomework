using System;
using System.Collections.Generic;
using System.Configuration;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ChatClient.Models;
using ChatClient.Services;
using ChatClient.ViewModels;
using ChatClient.Views;
using MVVMUtils;
using SimpleInjector;

namespace ChatClient;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var container = Bootstrap();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = container.GetInstance<MainView>();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static Func<BaseViewModel> CreateProducer<T>(Container container)
        where T : BaseViewModel => Lifestyle.Transient.CreateProducer<BaseViewModel, T>(container).GetInstance;

    private Container Bootstrap()
    {
        var container = new Container();
        container.Register<MainViewModel>(Lifestyle.Singleton);
        container.Register<MainView>(Lifestyle.Singleton);
        container.Register<INavigationStore<BaseViewModel>, NavigationStore>(Lifestyle.Singleton);
        container.Register<INavigationService<BaseViewModel>, NavigationService<BaseViewModel>>(Lifestyle.Singleton);
        container.RegisterInitializer<MainView>(x =>
        {
            x.DataContext = container.GetInstance<MainViewModel>();
        });
        container.Collection.Register<BaseViewModel>(typeof(BaseViewModel).Assembly);
        container.Register<IViewModelFactory<BaseViewModel>>(() =>
        {
            var factory = new ViewModelFactory<BaseViewModel>(new Dictionary<Type, Func<BaseViewModel>>()
            {
                [typeof(LoginViewModel)] = CreateProducer<LoginViewModel>(container),
                [typeof(ChatViewModel)] = CreateProducer<ChatViewModel>(container)
            });
            return factory;
        }, Lifestyle.Singleton);
        container.Register<IChatClient>(() => new MyChatClient("127.0.0.1", 27001, false));

        return container;
    }

}