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
                                                                 insert into movies (id, slug, title, yearofrelease)
                                                                 values (@id, @slug, @title, @yearofrelease);
                                                                 """, movie);
        var result = await connection.ExecuteAsync(insertMovieCommandDefinition);

        if (result > 0)
        {
            foreach (var genre in movie.Genres)
            {
                var insertGenreCommandDefinition = new CommandDefinition("""
                                                                         insert into genres (movieid, name)
                                                                         values (@movieId, @name);
                                                                         """, new { MovieId = movie.Id, Name = genre });
                await connection.ExecuteAsync(insertGenreCommandDefinition);
            }
        }
        
        transaction.Commit();

        return result > 0;
    }

    public async Task<Movie?> GetByIdAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        
        var getMovieCommandDefinition = new CommandDefinition("""
                                                              select
                                                                  *
                                                              from
                                                                  movies
                                                              where
                                                                  id = @id
                                                              """, new { Id = id });
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(getMovieCommandDefinition);
        
        if (movie is null)
        {
            return null;
        }

        var getGenresCommandDefinition = new CommandDefinition("""
                                                               select
                                                                   name
                                                               from
                                                                   genres
                                                               where
                                                                   movieid = @id
                                                               """, new { Id = id });
        var genres = await connection.QueryAsync<string>(getGenresCommandDefinition);

        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }
        
        return movie;
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