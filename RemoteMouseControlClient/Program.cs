using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using RemoteMouseControlClient;

class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Connect to somebody(conn)\nWait for connection(wait)\n3.Exit the program");
        var flag = false;
        while (!flag)
        {
            Console.Write("> ");
            var input = Console.ReadLine();
            switch (input)
            {
                case "conn":
                    await Connect();
                    // Console.WriteLine("Press space to finish the program..");
                    // while (Console.ReadKey().Key != ConsoleKey.Spacebar)
                    // {
                    // }
                    // cts.Cancel();
                    flag = true;
                    break;
                case "wait":
                    await WaitForConnection();
                    flag = true;
                    break;
                default:
                    continue;
            }
        }
    }
    
    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);
        
    [DllImport("user32.dll")]
    static extern bool GetCursorPos(ref Point lpPoint);


    private static async Task Connect()
    {
        var client = new UdpClient();
        client.Connect("de1.localtonet.com", 37691);
        await client.SendAsync(Encoding.UTF8.GetBytes("Sender"));
        while (true)
        {
            Console.WriteLine("Enter token: ");
            var input = Console.ReadLine();
            await client.SendAsync(Encoding.UTF8.GetBytes(input ?? string.Empty));
            var confirmationResult = await client.ReceiveAsync();
            var confirmation = Encoding.UTF8.GetString(confirmationResult.Buffer);
            if (confirmation == "True")
            {
                break;
            }
        }
        Console.WriteLine("You're connected! You can now control the mouse of the user you're connected to!");
        var cts = new CancellationTokenSource();
        var point = new Point();
        _ = Task.Run( async () =>
        {
            while (!cts.Token.IsCancellationRequested)
            {
                if (!GetCursorPos(ref point))
                    continue;
                var message = $"{point.X} {point.Y}";
                await client.SendAsync(Encoding.UTF8.GetBytes(message), cts.Token);
                Thread.Sleep(1);
            }
        }, cts.Token);
        Console.WriteLine("Press space to finish the program..");
        while (Console.ReadKey().Key != ConsoleKey.Spacebar)
        {
        }
        cts.Cancel();
    }

    private static async Task WaitForConnection()
    {
        var guid = Guid.NewGuid();
        var client = new UdpClient();
        client.Connect("de1.localtonet.com", 37691);
        await client.SendAsync(Encoding.UTF8.GetBytes("Receiver"));
        Console.WriteLine($"Your token: {guid.ToString()}");
        while (true)
        {
            var guidToken = await client.ReceiveAsync();
            if (Encoding.UTF8.GetString(guidToken.Buffer) != guid.ToString())
            {
                await client.SendAsync(Encoding.UTF8.GetBytes("False"));
                continue;
            }
            Console.WriteLine("Someone connected to you! The mouse controls are given to them!");
            var bytes = Encoding.UTF8.GetBytes("True");
            await client.SendAsync(bytes);
            break;
        }
        var cts = new CancellationTokenSource();
        _= Task.Run( async () =>
        {
            while (!cts.Token.IsCancellationRequested)
            {
                var salam = await client.ReceiveAsync(cts.Token);
                var pos = Encoding.UTF8.GetString(salam.Buffer).Split(' ').Select(x => Convert.ToInt32(x)).ToArray();
                SetCursorPos(pos[0], pos[1]);
                Thread.Sleep(1);
            }
        }, cts.Token);
        Console.WriteLine("Press space to finish the program..");
        while (Console.ReadKey().Key != ConsoleKey.Spacebar)
        {
        }
        cts.Cancel();
    }
    
}






