namespace CurrencyConverterServer;

public interface ICurrencyDbClient
{
    public Task<CurrencyConversion?> GetCurrencyConversion(Currency from, Currency to);
    public Task<Currency?> GetCurrency(string name);
}