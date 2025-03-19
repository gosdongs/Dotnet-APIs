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

    public async Task<Movie?> GetBySlugAsync(string slug)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        
        var getMovieCommandDefinition = new CommandDefinition("""
                                                              select
                                                                  *
                                                              from
                                                                  movies
                                                              where
                                                                  slug = @slug
                                                              """, new { Slug = slug });
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
                                                               """, new { Id = movie.Id });
        var genres = await connection.QueryAsync<string>(getGenresCommandDefinition);

        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }
        
        return movie;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync()
    {
        var connection = await _dbConnectionFactory.CreateConnectionAsync();

        var getAllMoviesCommandDefinition = new CommandDefinition("""
                                                                  select
                                                                      m.*, string_agg(g.name, ',') as genres
                                                                  from
                                                                      movies m
                                                                  left join
                                                                      genres g on g.movieid = m.id
                                                                  group by
                                                                      m.id
                                                                  """);
        var result = await connection.QueryAsync(getAllMoviesCommandDefinition);

        return result.Select(m => new Movie
        {
            Id = m.id,
            Title = m.title,
            YearOfRelease = m.yearofrelease,
            Genres = Enumerable.ToList(m.genres.Split(','))
        });
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