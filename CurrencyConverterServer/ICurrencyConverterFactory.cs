namespace CurrencyConverterServer;

public interface ICurrencyConverterFactory
{
    public ICurrencyConverter Create(string currency);
}