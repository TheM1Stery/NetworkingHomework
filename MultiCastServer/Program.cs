using System.Net;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MultiCastServer;


var serviceCollection = new ServiceCollection();

ConfigureServices(serviceCollection);

var serviceProvider = serviceCollection.BuildServiceProvider();

var server = serviceProvider.GetRequiredService<MultiCastSenderServer>();

_ = server.Start();



while (true)
{
    var str = Console.ReadLine();
    await server.Send(str);
}




static void ConfigureServices(IServiceCollection collection)
{
    collection
        .AddMediatR(Assembly.GetExecutingAssembly())
        .AddSingleton(IPEndPoint.Parse("127.0.0.1:34111"))
        .AddSingleton<MultiCastSenderServer>();
}