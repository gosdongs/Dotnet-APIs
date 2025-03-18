using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public MovieRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }
    
    public async Task<bool> CreateAsync(Movie movie)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        var insertMovieCommandDefinition = new CommandDefinition("""
                                                                 INSERT INTO Movies (Id, Slug, Title, YearOfRelease)
                                                                 VALUES (@Id, @Slug, @Title, @YearOfRelease);
                                                                 """, movie);
        var result = await connection.ExecuteAsync(insertMovieCommandDefinition);

        if (result > 0)
        {
            foreach (var genre in movie.Genres)
            {
                var insertGenreCommandDefinition = new CommandDefinition("""
                                                                         INSERT INTO Genres (MovieId, Name)
                                                                         VALUES (@MovieId, @Name);
                                                                         """, new { MovieId = movie.Id, Name = genre });
                await connection.ExecuteAsync(insertGenreCommandDefinition);
            }
        }
        
        transaction.Commit();

        return result > 0;
    }

    public Task<Movie?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Movie?> GetBySlugAsync(string slug)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Movie>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(Movie movie)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}