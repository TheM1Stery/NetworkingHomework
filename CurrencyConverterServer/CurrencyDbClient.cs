using System.Data.Common;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CurrencyConverterServer;

public class CurrencyDbClient : ICurrencyDbClient
{
    private readonly DbProviderFactory _factory;
    private readonly string _connectionString;
    
    
    public CurrencyDbClient(DbProviderFactory factory, string connectionString)
    {
        _factory = factory;
        _connectionString = connectionString;
        factory.CreateConnection();
    }
    
    
    public async Task<CurrencyConversion?> GetCurrencyConversion(Currency from, Currency to)
    {
        await using var connection = _factory.CreateConnection();
        connection!.ConnectionString = _connectionString;
        return await connection.QuerySingleOrDefaultAsync<CurrencyConversion>("SELECT [From], [To], OneCurrencyUnitCost " +
                                                             "AS Cost FROM CurrencyConversions WHERE [From] = @From " +
                                                       "AND [To] = @To", new {From = from.Id, To = to.Id});
    }

    public async Task<Currency?> GetCurrency(string name)
    {
        await using var connection = _factory.CreateConnection();
        connection!.ConnectionString = _connectionString;
        return await connection.QuerySingleAsync<Currency>("SELECT * FROM Currencies WHERE [Name] = @Name",
            new {Name = name});
    }
}