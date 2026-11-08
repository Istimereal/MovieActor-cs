using Microsoft.EntityFrameworkCore;
using MovieActorApi.Contracts;
using MovieActorApi.Data;
using MovieActorApi.Models;

namespace MovieActorApi.Services;

public enum LinkActorResult
{
    Linked,
    MovieNotFound,
    ActorNotFound,
    AlreadyLinked
}

public class MovieService
{
    private readonly AppDbContext _db;

    public MovieService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<MovieResponse>> GetAllAsync()
    {
        // Include loads each MovieActor row, ThenInclude loads the Actor on that row.
        var movies = await _db.Movies
            .Include(movie => movie.MovieActors)
            .ThenInclude(link => link.Actor)
            .OrderBy(movie => movie.Title)
            .ToListAsync();

        return movies.Select(ToResponse).ToList();
    }

    public async Task<MovieResponse?> GetByIdAsync(Guid id)
    {
        var movie = await _db.Movies
            .Include(m => m.MovieActors)
            .ThenInclude(link => link.Actor)
            .FirstOrDefaultAsync(m => m.Id == id);

        return movie is null ? null : ToResponse(movie);
    }

    public async Task<MovieResponse> CreateAsync(CreateMovieRequest request)
    {
        var movie = new Movie
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            DurationMinutes = request.DurationMinutes,
            ReleaseYear = request.ReleaseYear
        };

        _db.Movies.Add(movie);
        await _db.SaveChangesAsync();

        return ToResponse(movie);
    }

    public async Task<MovieResponse?> UpdateAsync(Guid id, UpdateMovieRequest request)
    {
        var movie = await _db.Movies
            .Include(m => m.MovieActors)
            .ThenInclude(link => link.Actor)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (movie is null)
        {
            return null;
        }

        movie.Title = request.Title.Trim();
        movie.DurationMinutes = request.DurationMinutes;
        movie.ReleaseYear = request.ReleaseYear;
        await _db.SaveChangesAsync();

        return ToResponse(movie);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var movie = await _db.Movies.FindAsync(id);
        if (movie is null)
        {
            return false;
        }

        _db.Movies.Remove(movie);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<LinkActorResult> LinkActorAsync(Guid movieId, Guid actorId)
    {
        var movieExists = await _db.Movies.AnyAsync(movie => movie.Id == movieId);
        if (!movieExists)
        {
            return LinkActorResult.MovieNotFound;
        }

        var actorExists = await _db.Actors.AnyAsync(actor => actor.Id == actorId);
        if (!actorExists)
        {
            return LinkActorResult.ActorNotFound;
        }

        var alreadyLinked = await _db.MovieActors.AnyAsync(link =>
            link.MovieId == movieId && link.ActorId == actorId);
        if (alreadyLinked)
        {
            return LinkActorResult.AlreadyLinked;
        }

        _db.MovieActors.Add(new MovieActor
        {
            MovieId = movieId,
            ActorId = actorId
        });
        await _db.SaveChangesAsync();
        return LinkActorResult.Linked;
    }

    public async Task<bool> UnlinkActorAsync(Guid movieId, Guid actorId)
    {
        var link = await _db.MovieActors.FindAsync(movieId, actorId);
        if (link is null)
        {
            return false;
        }

        _db.MovieActors.Remove(link);
        await _db.SaveChangesAsync();
        return true;
    }

    private static MovieResponse ToResponse(Movie movie)
    {
        var actors = movie.MovieActors
            .OrderBy(link => link.Actor.Name)
            .Select(link => new ActorResponse(
                link.Actor.Id,
                link.Actor.Name,
                link.Actor.Age,
                link.Actor.Popularity))
            .ToList();

        return new MovieResponse(
            movie.Id,
            movie.Title,
            movie.DurationMinutes,
            movie.ReleaseYear,
            actors);
    }
}
