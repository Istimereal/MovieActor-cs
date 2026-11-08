namespace MovieActorApi.Models;

public class Movie
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public int ReleaseYear { get; set; }

    public List<MovieActor> MovieActors { get; set; } = new();
}
