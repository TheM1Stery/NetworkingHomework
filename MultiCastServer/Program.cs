using System.Net;
using MultiCastServer;




var server = new MultiCastSenderServer(IPEndPoint.Parse("127.0.0.1:34111"));

_ = server.Start();



while (true)
{
    var str = Console.ReadLine();
    await server.Send(str);
}