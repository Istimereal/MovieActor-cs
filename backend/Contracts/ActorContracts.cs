using System.ComponentModel.DataAnnotations;

namespace MovieActorApi.Contracts;

public record ActorResponse(
    Guid Id,
    string Name,
    int Age,
    int Popularity);

public record ActorDetailResponse(
    Guid Id,
    string Name,
    int Age,
    int Popularity,
    List<MovieSummary> Movies);

public record CreateActorRequest(
    [property: Required] string Name,
    int Age,
    int Popularity);

public record UpdateActorRequest(
    [property: Required] string Name,
    int Age,
    int Popularity);
