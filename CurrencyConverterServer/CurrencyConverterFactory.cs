namespace CurrencyConverterServer;

public class CurrencyConverterFactory : ICurrencyConverterFactory
{
    public ICurrencyConverter Create(string currency)
    {
        return currency switch
        {
            _ => throw new ArgumentException("Provided currency is not supported")
        };
    }
}