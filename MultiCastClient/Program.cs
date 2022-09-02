// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;

var client = new TcpClient();

await client.ConnectAsync(IPEndPoint.Parse("127.0.0.1:34111"));

var stream = new StreamReader(client.GetStream());


await Task.Run(() =>
{
    while (true)
    {
        var message = stream.ReadLine();
        Console.WriteLine(message);
    }
});
