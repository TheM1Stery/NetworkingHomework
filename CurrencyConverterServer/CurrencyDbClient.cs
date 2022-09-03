using Dapper;
using Microsoft.Data.SqlClient;

namespace CurrencyConverterServer;

public class CurrencyDbClient : ICurrencyDbClient
{
    private readonly string _connectionString;
    
    
    public CurrencyDbClient(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    
    public async Task<CurrencyConversion?> GetCurrencyConversion(Currency from, Currency to)
    {
        await using var connection = new SqlConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<CurrencyConversion>("SELECT [From], [To], OneCurrencyUnitCost " +
                                                             "AS Cost FROM CurrencyConversions WHERE [From] = @From " +
                                                       "AND [To] = @To", new {From = from, To = to});
        
    }

    public async Task<Currency?> GetCurrency(string name)
    {
        await using var connection = new SqlConnection(_connectionString);
        return await connection.QuerySingleAsync<Currency>("SELECT * FROM Currencies WHERE [Name] = @Name",
            new {Name = name});
    }
}