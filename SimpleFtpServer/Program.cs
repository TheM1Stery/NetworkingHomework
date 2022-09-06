using System.Net;
using SimpleFtpServer;

var server = new Server(IPEndPoint.Parse("127.0.0.1:13377"), "FtpDirectory");

var cts = new CancellationTokenSource();

server.Start(cts.Token).FireAndForget();

Console.WriteLine("Press anything to close the server..");
Console.ReadLine();
cts.Cancel();