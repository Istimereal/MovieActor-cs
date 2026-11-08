using Microsoft.EntityFrameworkCore;
using MovieActorApi.Models;

namespace MovieActorApi.Data;

// AppDbContext is the EF Core session. Services use it to query and save PostgreSQL.
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Actor> Actors => Set<Actor>();
    public DbSet<MovieActor> MovieActors => Set<MovieActor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>(movie =>
        {
            movie.HasKey(m => m.Id);
            movie.Property(m => m.Title).IsRequired();
        });

        modelBuilder.Entity<Actor>(actor =>
        {
            actor.HasKey(a => a.Id);
            actor.Property(a => a.Name).IsRequired();
        });

        modelBuilder.Entity<MovieActor>(link =>
        {
            // Composite primary key: the same movie and actor can be linked only once.
            link.HasKey(x => new { x.MovieId, x.ActorId });

            link.HasOne(x => x.Movie)
                .WithMany(movie => movie.MovieActors)
                .HasForeignKey(x => x.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            link.HasOne(x => x.Actor)
                .WithMany(actor => actor.MovieActors)
                .HasForeignKey(x => x.ActorId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
