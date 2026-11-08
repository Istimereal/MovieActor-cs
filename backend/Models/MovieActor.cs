namespace MovieActorApi.Models;

// MovieActor is the join row for the many-to-many link: one movie, one actor.
public class MovieActor
{
    public Guid MovieId { get; set; }
    public Guid ActorId { get; set; }

    public Movie Movie { get; set; } = null!;
    public Actor Actor { get; set; } = null!;
}
