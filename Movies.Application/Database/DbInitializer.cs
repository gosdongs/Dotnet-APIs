using Dapper;

namespace Movies.Application.Database;

public class DbInitializer
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public DbInitializer(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task InitializeAsync()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        await connection.ExecuteAsync("""
                                      CREATE TABLE IF NOT EXISTS Movies (
                                          Id UUID PRIMARY KEY,
                                          Slug TEXT NOT NULL,
                                          Title TEXT NOT NULL,
                                          YearOfRelease INTEGER NOT NULL
                                      )
                                      """);
    }
}