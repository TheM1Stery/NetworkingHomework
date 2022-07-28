using ChatServer;

var resetEvent = new ManualResetEvent(false);


using var server = new MyChatServer("127.0.0.1", 27001);
server.Disconnected += () =>
{
    Console.WriteLine("Served ended");
    resetEvent.Set();
};

var tokenSource = new CancellationTokenSource();

Console.WriteLine("Press any button to start...");
Console.ReadLine();

_ = server.StartInfiniteAsync(tokenSource.Token);

Console.WriteLine("Press any button to end server");
Console.ReadLine();
tokenSource.Cancel();
resetEvent.WaitOne();