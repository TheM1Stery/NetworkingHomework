using System.Net;
using CurrencyConverterServer;
using Microsoft.Data.SqlClient;
using Serilog;

using var logger = new LoggerConfiguration().WriteTo.File("log.txt").WriteTo.Console().CreateLogger();


var client = new MyCurrencyConverterServer(IPEndPoint.Parse("127.0.0.1:13377"), new CurrencyDbClient(SqlClientFactory.Instance, 
    @"Server=(localdb)\MSSQLLocalDB; Database=CurrencyDB"), logger);


var cts = new CancellationTokenSource();

var task = client.Start(cts.Token);

Console.WriteLine("Press anything to end the server...");
Console.ReadLine();
cts.Cancel();