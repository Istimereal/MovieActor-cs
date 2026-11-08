namespace MovieActorApi.Models;

public class Actor
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public int Popularity { get; set; }

    public List<MovieActor> MovieActors { get; set; } = new();
}
