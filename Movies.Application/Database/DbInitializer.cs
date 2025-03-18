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
                                      );
                                      """);

        await connection.ExecuteAsync("""
                                      CREATE UNIQUE INDEX CONCURRENTLY IF NOT EXISTS
                                        Movies_Slug_Idx
                                      ON
                                        Movies
                                      USING BTREE(Slug);
                                      """);

        await connection.ExecuteAsync("""
                                      CREATE TABLE IF NOT EXISTS Genres (
                                          MovieId UUID REFERENCES Movies (Id),
                                          Name TEXT NOT NULL
                                      );
                                      """);
    }
}