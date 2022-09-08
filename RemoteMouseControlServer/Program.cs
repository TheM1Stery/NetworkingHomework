// See https://aka.ms/new-console-template for more information

using System.Net;
using RemoteMouseControlServer;

using var server = new Server(IPEndPoint.Parse("127.0.0.1:13377"));

await server.ReceiveClients();


await server.Start();