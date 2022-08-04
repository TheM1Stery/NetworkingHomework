using System.Net;
using System.Net.Sockets;

namespace CurrencyConverterServer;

public class MyCurrencyConverterServer
{
    private readonly ICurrencyConverterFactory _factory;
    private TcpListener _server;

    public MyCurrencyConverterServer(string ip, int port, ICurrencyConverterFactory factory)
    {
        _factory = factory;
        _server = new TcpListener(IPAddress.Parse(ip), port);
    }
}