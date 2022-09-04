namespace CurrencyConverterServer;

public record CurrencyConversion
{
    public decimal Cost { get; init; }
    public string? To { get; init; }
    public string? From { get; init; } 
}