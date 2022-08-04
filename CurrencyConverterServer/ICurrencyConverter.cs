namespace CurrencyConverterServer;

public interface ICurrencyConverter
{
    /// <param name="money">The money that will be converted</param>
    /// <param name="fromCurrency">the currency of the money</param>
    /// <returns>the converted money</returns>
    public decimal Convert(decimal money, string fromCurrency);
}