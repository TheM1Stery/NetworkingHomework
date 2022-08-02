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

    private Container? _container;

    public override void OnFrameworkInitializationCompleted()
    {
        Bootstrap();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = _container?.GetInstance<MainView>();
        }
        base.OnFrameworkInitializationCompleted();
    }

    private static Func<BaseViewModel> CreateProducer<T>(Container container)
        where T : BaseViewModel => Lifestyle.Transient.CreateProducer<BaseViewModel, T>(container).GetInstance;

    private void Bootstrap()
    {
        _container = new Container();
        _container.Register<MainViewModel>(Lifestyle.Singleton);
        _container.Register<MainView>(Lifestyle.Singleton);
        _container.Register<INavigationStore<BaseViewModel>, NavigationStore>(Lifestyle.Singleton);
        _container.Register<INavigationService<BaseViewModel>, NavigationService<BaseViewModel>>(Lifestyle.Singleton);
        _container.RegisterInitializer<MainView>(x =>
        {
            x.DataContext = _container.GetInstance<MainViewModel>();
        });
        _container.Collection.Register<BaseViewModel>(typeof(BaseViewModel).Assembly);
        _container.Register<IViewModelFactory<BaseViewModel>>(() =>
        {
            var factory = new ViewModelFactory<BaseViewModel>(new Dictionary<Type, Func<BaseViewModel>>()
            {
                [typeof(LoginViewModel)] = CreateProducer<LoginViewModel>(_container),
                [typeof(ChatViewModel)] = CreateProducer<ChatViewModel>(_container)
            });
            return factory;
        }, Lifestyle.Singleton);
        _container.Register<IChatClient>(() => new MyChatClient("127.0.0.1", 27001, false));
        // _container.Register<IChatClient>(() => new MyChatClient("5.tcp.eu.ngrok.io", 13162, 
        //     true));
    }

}