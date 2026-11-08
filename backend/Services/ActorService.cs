using Microsoft.EntityFrameworkCore;
using MovieActorApi.Contracts;
using MovieActorApi.Data;
using MovieActorApi.Models;

namespace MovieActorApi.Services;

public class ActorService
{
    private readonly AppDbContext _db;

    public ActorService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ActorResponse>> GetAllAsync()
    {
        return await _db.Actors
            .OrderBy(actor => actor.Name)
            .Select(actor => new ActorResponse(
                actor.Id,
                actor.Name,
                actor.Age,
                actor.Popularity))
            .ToListAsync();
    }

    public async Task<ActorDetailResponse?> GetByIdAsync(Guid id)
    {
        // Include loads each MovieActor row, ThenInclude loads the Movie on that row.
        var actor = await _db.Actors
            .Include(a => a.MovieActors)
            .ThenInclude(link => link.Movie)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (actor is null)
        {
            return null;
        }

        var movies = actor.MovieActors
            .OrderBy(link => link.Movie.Title)
            .Select(link => new MovieSummary(
                link.Movie.Id,
                link.Movie.Title,
                link.Movie.DurationMinutes,
                link.Movie.ReleaseYear))
            .ToList();

        return new ActorDetailResponse(
            actor.Id,
            actor.Name,
            actor.Age,
            actor.Popularity,
            movies);
    }

    public async Task<ActorResponse> CreateAsync(CreateActorRequest request)
    {
        var actor = new Actor
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Age = request.Age,
            Popularity = request.Popularity
        };

        _db.Actors.Add(actor);
        await _db.SaveChangesAsync();

        return new ActorResponse(actor.Id, actor.Name, actor.Age, actor.Popularity);
    }

    public async Task<ActorResponse?> UpdateAsync(Guid id, UpdateActorRequest request)
    {
        var actor = await _db.Actors.FindAsync(id);
        if (actor is null)
        {
            return null;
        }

        actor.Name = request.Name.Trim();
        actor.Age = request.Age;
        actor.Popularity = request.Popularity;
        await _db.SaveChangesAsync();

        return new ActorResponse(actor.Id, actor.Name, actor.Age, actor.Popularity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var actor = await _db.Actors.FindAsync(id);
        if (actor is null)
        {
            return false;
        }

        _db.Actors.Remove(actor);
        await _db.SaveChangesAsync();
        return true;
    }
}
