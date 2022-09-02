// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;

using var client = new TcpClient();

var time = TimeOnly.FromDateTime(DateTime.Now);


var connected = false;
Console.WriteLine("Waiting for the server...");
while (!connected)
{
    if ((TimeOnly.FromDateTime(DateTime.Now) - time).Seconds > 15)
    {
        Console.WriteLine("Couldn't connect to the server. Closing the program...");
        return;
    }
    try
    {
        await client.ConnectAsync(IPEndPoint.Parse("127.0.0.1:34111"));
        connected = true;
        Console.WriteLine("Connected!");
    }
    catch (Exception)
    {
        connected = false;
    }
    Thread.Sleep(1);
}

var stream = new StreamReader(client.GetStream());


await Task.Run(() =>
{
    try
    {
        while (true)
        {
            var message = stream.ReadLine();
            Console.WriteLine($"Message sent by the server: {message}");
        }
    }
    catch (Exception)
    {
        Console.WriteLine("Server closed connection. Press anything to close the program...");
        Console.ReadLine();
    }
    
});


