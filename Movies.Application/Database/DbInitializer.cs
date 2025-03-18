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
                                      create table if not exists movies (
                                          id uuid primary key,
                                          slug text not null,
                                          title text not null,
                                          yearofrelease integer not null
                                      );
                                      """);

        await connection.ExecuteAsync("""
                                      create unique index concurrently if not exists
                                          movies_slug_idx
                                      on
                                          movies
                                      using btree(slug);
                                      """);

        await connection.ExecuteAsync("""
                                      create table if not exists genres (
                                          movieid uuid references movies (id),
                                          name text not null
                                      );
                                      """);
    }
}