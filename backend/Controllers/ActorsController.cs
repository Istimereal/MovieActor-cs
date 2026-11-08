using Microsoft.AspNetCore.Mvc;
using MovieActorApi.Contracts;
using MovieActorApi.Services;

namespace MovieActorApi.Controllers;

[ApiController]
[Route("api/actors")]
public class ActorsController : ControllerBase
{
    private readonly ActorService _actors;

    public ActorsController(ActorService actors)
    {
        _actors = actors;
    }

    [HttpGet]
    public async Task<ActionResult<List<ActorResponse>>> GetAll()
    {
        return Ok(await _actors.GetAllAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ActorDetailResponse>> GetById(Guid id)
    {
        var actor = await _actors.GetByIdAsync(id);
        if (actor is null)
        {
            return NotFound();
        }

        return Ok(actor);
    }

    [HttpPost]
    public async Task<ActionResult<ActorResponse>> Create(CreateActorRequest request)
    {
        var actor = await _actors.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = actor.Id }, actor);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ActorResponse>> Update(Guid id, UpdateActorRequest request)
    {
        var actor = await _actors.UpdateAsync(id, request);
        if (actor is null)
        {
            return NotFound();
        }

        return Ok(actor);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _actors.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
