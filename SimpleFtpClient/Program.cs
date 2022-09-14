using System.Net;
using System.Net.Sockets;


using var client = new TcpClient();

await client.ConnectAsync(IPEndPoint.Parse("127.0.0.1:13377"));


var stream = client.GetStream();
var binaryReader = new BinaryReader(stream);
var binaryWriter = new BinaryWriter(stream);
var textReader = new StreamReader(stream);
var textWriter = new StreamWriter(stream){AutoFlush = true};

await textWriter.WriteLineAsync("ls");
var str = await textReader.ReadLineAsync();
Console.WriteLine(str);
await textWriter.WriteLineAsync("get helloworld.png");
await using var filestream = new FileStream("helloworld2.png", FileMode.OpenOrCreate, FileAccess.Write);
var length = binaryReader.ReadInt32();
for (var i = 0; i < length; i++)
{
    filestream.WriteByte(binaryReader.ReadByte());    
}
Console.WriteLine("File downloaded");

var bytes = File.ReadAllBytes("cat.jpg");
await textWriter.WriteLineAsync("upload cat.jpg");
binaryWriter.Write(bytes.Length);
binaryWriter.Flush();
binaryWriter.Write(bytes);
binaryWriter.Flush();