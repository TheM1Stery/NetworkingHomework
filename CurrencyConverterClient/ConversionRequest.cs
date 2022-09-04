namespace CurrencyConverterClient;

public record ConversionRequest(string From, string To, decimal MoneyToConvert);