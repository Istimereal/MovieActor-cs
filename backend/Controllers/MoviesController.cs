using Microsoft.AspNetCore.Mvc;
using MovieActorApi.Contracts;
using MovieActorApi.Services;

namespace MovieActorApi.Controllers;

[ApiController]
[Route("api/movies")]
public class MoviesController : ControllerBase
{
    private readonly MovieService _movies;

    public MoviesController(MovieService movies)
    {
        _movies = movies;
    }

    [HttpGet]
    public async Task<ActionResult<List<MovieResponse>>> GetAll()
    {
        return Ok(await _movies.GetAllAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MovieResponse>> GetById(Guid id)
    {
        var movie = await _movies.GetByIdAsync(id);
        if (movie is null)
        {
            return NotFound();
        }

        return Ok(movie);
    }

    [HttpPost]
    public async Task<ActionResult<MovieResponse>> Create(CreateMovieRequest request)
    {
        var movie = await _movies.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = movie.Id }, movie);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<MovieResponse>> Update(Guid id, UpdateMovieRequest request)
    {
        var movie = await _movies.UpdateAsync(id, request);
        if (movie is null)
        {
            return NotFound();
        }

        return Ok(movie);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _movies.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost("{movieId:guid}/actors/{actorId:guid}")]
    public async Task<IActionResult> LinkActor(Guid movieId, Guid actorId)
    {
        var result = await _movies.LinkActorAsync(movieId, actorId);

        if (result == LinkActorResult.MovieNotFound)
        {
            return NotFound(new { message = "Movie not found." });
        }

        if (result == LinkActorResult.ActorNotFound)
        {
            return NotFound(new { message = "Actor not found." });
        }

        if (result == LinkActorResult.AlreadyLinked)
        {
            return Conflict(new { message = "This actor is already linked to the movie." });
        }

        return NoContent();
    }

    [HttpDelete("{movieId:guid}/actors/{actorId:guid}")]
    public async Task<IActionResult> UnlinkActor(Guid movieId, Guid actorId)
    {
        var removed = await _movies.UnlinkActorAsync(movieId, actorId);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}
