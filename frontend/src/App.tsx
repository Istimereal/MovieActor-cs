import { useEffect, useState, type FormEvent } from "react";
import "./App.css";

const apiUrl = "http://localhost:5080";

type Actor = {
  id: string;
  name: string;
  age: number;
  popularity: number;
};

type Movie = {
  id: string;
  title: string;
  durationMinutes: number;
  releaseYear: number;
  actors: Actor[];
};

export default function App() {
  const [movies, setMovies] = useState<Movie[]>([]);
  const [actors, setActors] = useState<Actor[]>([]);
  const [error, setError] = useState<string | null>(null);

  const [title, setTitle] = useState("");
  const [durationMinutes, setDurationMinutes] = useState("");
  const [releaseYear, setReleaseYear] = useState("");

  const [name, setName] = useState("");
  const [age, setAge] = useState("");
  const [popularity, setPopularity] = useState("");

  const [movieId, setMovieId] = useState("");
  const [actorId, setActorId] = useState("");

  async function loadLists() {
    const [moviesResponse, actorsResponse] = await Promise.all([
      fetch(`${apiUrl}/api/movies`),
      fetch(`${apiUrl}/api/actors`),
    ]);

    if (!moviesResponse.ok || !actorsResponse.ok) {
      throw new Error("Could not load movies and actors from the API.");
    }

    const nextMovies = (await moviesResponse.json()) as Movie[];
    const nextActors = (await actorsResponse.json()) as Actor[];
    setMovies(nextMovies);
    setActors(nextActors);
    setMovieId((current) => current || nextMovies[0]?.id || "");
    setActorId((current) => current || nextActors[0]?.id || "");
  }

  useEffect(() => {
    loadLists().catch((err: Error) => setError(err.message));
  }, []);

  async function createMovie(event: FormEvent) {
    event.preventDefault();
    setError(null);

    try {
      const response = await fetch(`${apiUrl}/api/movies`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          title,
          durationMinutes: Number(durationMinutes),
          releaseYear: Number(releaseYear),
        }),
      });

      if (!response.ok) {
        setError("Could not create the movie.");
        return;
      }

      setTitle("");
      setDurationMinutes("");
      setReleaseYear("");
      await loadLists();
    } catch {
      setError("The API did not respond. Start the backend, then try again.");
    }
  }

  async function createActor(event: FormEvent) {
    event.preventDefault();
    setError(null);

    try {
      const response = await fetch(`${apiUrl}/api/actors`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          name,
          age: Number(age),
          popularity: Number(popularity),
        }),
      });

      if (!response.ok) {
        setError("Could not create the actor.");
        return;
      }

      setName("");
      setAge("");
      setPopularity("");
      await loadLists();
    } catch {
      setError("The API did not respond. Start the backend, then try again.");
    }
  }

  async function connectActor(event: FormEvent) {
    event.preventDefault();
    setError(null);

    try {
      const response = await fetch(
        `${apiUrl}/api/movies/${movieId}/actors/${actorId}`,
        { method: "POST" },
      );

      if (response.status === 409) {
        setError("That actor is already connected to the movie.");
        return;
      }

      if (!response.ok) {
        setError("Could not connect the actor to the movie.");
        return;
      }

      await loadLists();
    } catch {
      setError("The API did not respond. Start the backend, then try again.");
    }
  }

  return (
    <main>
      <h1>Movies and actors</h1>
      {error && <p className="error">{error}</p>}

      <section>
        <h2>Movies</h2>
        {movies.length === 0 ? (
          <p>No movies yet.</p>
        ) : (
          <ul>
            {movies.map((movie) => (
              <li key={movie.id}>
                <strong>{movie.title}</strong>
                <span className="meta">
                  {" "}
                  {movie.durationMinutes} min, {movie.releaseYear}
                </span>
                <div>
                  {movie.actors.length === 0
                    ? "No actors yet"
                    : movie.actors.map((actor) => actor.name).join(", ")}
                </div>
              </li>
            ))}
          </ul>
        )}

        <form onSubmit={createMovie}>
          <label>
            Title
            <input
              value={title}
              onChange={(event) => setTitle(event.target.value)}
              required
            />
          </label>
          <label>
            Duration (minutes)
            <input
              type="number"
              value={durationMinutes}
              onChange={(event) => setDurationMinutes(event.target.value)}
              required
            />
          </label>
          <label>
            Release year
            <input
              type="number"
              value={releaseYear}
              onChange={(event) => setReleaseYear(event.target.value)}
              required
            />
          </label>
          <button type="submit">Add movie</button>
        </form>
      </section>

      <section>
        <h2>Actors</h2>
        {actors.length === 0 ? (
          <p>No actors yet.</p>
        ) : (
          <ul>
            {actors.map((actor) => (
              <li key={actor.id}>
                <strong>{actor.name}</strong>
                <span className="meta">
                  {" "}
                  age {actor.age}, popularity {actor.popularity}
                </span>
              </li>
            ))}
          </ul>
        )}

        <form onSubmit={createActor}>
          <label>
            Name
            <input
              value={name}
              onChange={(event) => setName(event.target.value)}
              required
            />
          </label>
          <label>
            Age
            <input
              type="number"
              value={age}
              onChange={(event) => setAge(event.target.value)}
              required
            />
          </label>
          <label>
            Popularity
            <input
              type="number"
              value={popularity}
              onChange={(event) => setPopularity(event.target.value)}
              required
            />
          </label>
          <button type="submit">Add actor</button>
        </form>
      </section>

      <section>
        <h2>Connect</h2>
        <form onSubmit={connectActor}>
          <label>
            Movie
            <select
              value={movieId}
              onChange={(event) => setMovieId(event.target.value)}
            >
              {movies.map((movie) => (
                <option key={movie.id} value={movie.id}>
                  {movie.title}
                </option>
              ))}
            </select>
          </label>
          <label>
            Actor
            <select
              value={actorId}
              onChange={(event) => setActorId(event.target.value)}
            >
              {actors.map((actor) => (
                <option key={actor.id} value={actor.id}>
                  {actor.name}
                </option>
              ))}
            </select>
          </label>
          <button type="submit" disabled={movieId === "" || actorId === ""}>
            Connect
          </button>
        </form>
      </section>
    </main>
  );
}
