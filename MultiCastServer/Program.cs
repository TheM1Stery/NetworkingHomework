using System.Net;
using MultiCastServer;




var server = new MultiCastSenderServer(IPEndPoint.Parse("127.0.0.1:34111"));

var cts = new CancellationTokenSource();

_ = server.Start(cts.Token);


var showMenu = true;


while (true)
{
    if (showMenu)
    {
        await Task.Delay(1200);
        Console.Clear();
        Console.WriteLine("Send person class object to client(person)\nSend a message(msg)\nExit(exit)");
    }
    Console.Write("> ");
    switch (Console.ReadLine())
    {
        case "person":
            await SendPerson();
            showMenu = true;
            break;
        case "msg":
            await SendMessage();
            showMenu = true;
            break;
        case "exit":
            cts.Cancel();
            return;
        default:
            showMenu = false;
            break;
    }
}


async Task SendPerson()
{
    Console.Write("Enter first name: \n> ");
    var firstName = Console.ReadLine();
    Console.Write("Enter last name: \n> ");
    var lastName = Console.ReadLine();
    await server.Send(new Person(firstName, lastName));
}

async Task SendMessage()
{
    Console.Write("Enter message: \n> ");
    var msg = Console.ReadLine();
    await server.Send(msg);
}


internal record Person(string? FirstName, string? LastName);