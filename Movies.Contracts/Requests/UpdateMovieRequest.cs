namespace Movies.Contracts.Requests;

public class UpdateMovieRequest
{
    public required string Title { get; init; }
    public required string Slug { get; set; }
    public required int YearOfRelease { get; init; }
    public required IEnumerable<string> Genres { get; init; } = [];
}