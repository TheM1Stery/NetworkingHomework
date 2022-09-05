// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using CurrencyConverterClient;

using var client = new TcpClient();

try
{
    await client.ConnectAsync(IPEndPoint.Parse("127.0.0.1:13377"));
}
catch (SocketException)
{
    Console.WriteLine("Couldn't connect to the server. Closing the program");
    await Task.Delay(1500);
    return;
}

await using var writer = new StreamWriter(client.GetStream())
{
    AutoFlush = true
};
var showMenu = true;
using var reader = new StreamReader(client.GetStream());
try
{
    var status = await reader.ReadLineAsync();
    if (status == "Server is loaded")
    {
        Console.WriteLine($"{status}. Please try again later. Closing the program");
        Console.ReadLine();
        return;
    }
}
catch (Exception)
{
    Console.WriteLine("Server disconnected.. Closing the program");
    await Task.Delay(3000);
}


var set = new HashSet<string>() { "azn", "eur", "usd", "uah" };
try
{
    while (true)
    {
        if (showMenu)
        {
            Console.WriteLine("Example: azn to uah 2");
            Console.WriteLine("AZN(azn)\nEUR(eur)\nUSD(usd)\nUAH(uah)\nClose the program(close)");
        }
        Console.Write("> ");
        var str = Console.ReadLine();
        if (str == "close")
        {
            return;
        }
        var arguments = str?.Split(' ');
        if (arguments is null)
        {
            showMenu = false;
            continue;
        }
        if (!ValidateArguments(arguments, set))
        {
            showMenu = false;
            continue;
        }
        showMenu = true;
        var request = new ConversionRequest(arguments[0].ToUpper(), arguments[2].ToUpper(), 
            decimal.Parse(arguments[3]));
        await HandleRequest(writer, reader, request);
    }
}
catch (Exception)
{
    Console.WriteLine("Server disconnected.. Closing the program");
    await Task.Delay(3000);
    return;
}


async Task HandleRequest(TextWriter streamWriter, TextReader streamReader, ConversionRequest request)
{
    await streamWriter.WriteLineAsync(JsonSerializer.Serialize(request));
    var json = await streamReader.ReadLineAsync();
    if (json == "Request limit exceeded. Closing the connection")
    {
        Console.WriteLine("Request limit exceeded. Closing the connection");
        await Task.Delay(3000);
        throw new InvalidOperationException();
    }
    var memStream = new MemoryStream(Encoding.UTF8.GetBytes(json!));
    var conversionResult = await JsonSerializer.DeserializeAsync<ConversionResult>(memStream);
    Console.Clear();
    Console.WriteLine($"Result of conversion from {request.MoneyToConvert} {request.From} " +
                      $"to {request.To} = {conversionResult?.Result}");
}

bool ValidateArguments(IReadOnlyList<string> args, IReadOnlySet<string> filters)
{
    if (args.Count is not 4)
    {
        return false;
    }

    if (args[0] == args[2])
        return false;

    if (!filters.Contains(args[0]))
        return false;

    if (args[1] != "to")
        return false;

    if (!filters.Contains(args[2]))
        return false;

    return args[3].All(char.IsDigit);
}
