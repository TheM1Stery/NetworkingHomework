// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using CurrencyConverterClient;

using var client = new TcpClient();

await client.ConnectAsync(IPEndPoint.Parse("127.0.0.1:13377"));

await using var writer = new StreamWriter(client.GetStream())
{
    AutoFlush = true
};
var showMenu = false;
using var reader = new StreamReader(client.GetStream());
while (true)
{
    if (showMenu is true)
    {
        Console.Write("AZN(azn)\nEUR(eur)\n3.USD(usd)\n4.UAH(uah)\n> ");
    }
    var str = Console.ReadLine();
    var request = new ConversionRequest("EUR", "AZN", 3m);
    await HandleRequest(writer, reader, request);
}

async Task HandleRequest(StreamWriter streamWriter, StreamReader streamReader, ConversionRequest request)
{
    await streamWriter.WriteLineAsync(JsonSerializer.Serialize(request));
    var json = await streamReader.ReadLineAsync();
    var memStream = new MemoryStream(Encoding.UTF8.GetBytes(json!));
    var conversionResult = await JsonSerializer.DeserializeAsync<ConversionResult>(memStream);
    Console.WriteLine($"Result of conversion from {request.MoneyToConvert} {request.From} " +
                      $"to {request.To} = {conversionResult?.Result}");
}

