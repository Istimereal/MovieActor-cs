using System.ComponentModel.DataAnnotations;

namespace MovieActorApi.Contracts;

public record MovieResponse(
    Guid Id,
    string Title,
    int DurationMinutes,
    int ReleaseYear,
    List<ActorResponse> Actors);

public record MovieSummary(
    Guid Id,
    string Title,
    int DurationMinutes,
    int ReleaseYear);

public record CreateMovieRequest(
    [property: Required] string Title,
    int DurationMinutes,
    int ReleaseYear);

public record UpdateMovieRequest(
    [property: Required] string Title,
    int DurationMinutes,
    int ReleaseYear);
