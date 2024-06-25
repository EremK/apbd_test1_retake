using Microsoft.Data.SqlClient;
using WebApplication1.DTOs;

namespace WebApplication1.Repositories;

public interface IMovieRepository
{
    Task<List<MovieDetailsDto>> GetAllMoviesAsync(DateTime? releaseDate);
    Task<bool> AssignActorToMovieAsync(int movieId, int actorId, string characterName);
}

public class MovieRepository : IMovieRepository
{
    private readonly string _connectionString =
        "Server=db-mssql;Initial Catalog=2019SBD;User ID=;Password=;Integrated Security=False;Trust Server Certificate=True; Trusted_Connection=true;encrypt=false;";

    public async Task<List<MovieDetailsDto>> GetAllMoviesAsync(DateTime? releaseDate)
    {
        var movies = new List<MovieDetailsDto>();
        var movieDict = new Dictionary<int, MovieDetailsDto>();

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var query = @"
            SELECT m.IdMovie, m.Name, m.ReleaseDate, ar.Name AS AgeRating, a.IdActor, a.Name AS ActorName, a.Surname, am.CharacterName 
            FROM Movie m
            INNER JOIN AgeRating ar ON m.IdAgeRating = ar.IdRating
            LEFT JOIN Actor_Movie am ON am.IdMovie = m.IdMovie
            LEFT JOIN Actor a ON a.IdActor = am.IdActor
            " + (releaseDate.HasValue ? "WHERE m.ReleaseDate = @ReleaseDate " : "") +
                        "ORDER BY m.ReleaseDate DESC";

            using (var command = new SqlCommand(query, connection))
            {
                if (releaseDate.HasValue) command.Parameters.AddWithValue("@ReleaseDate", releaseDate.Value);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var movieId = (int)reader["IdMovie"];
                        if (!movieDict.TryGetValue(movieId, out var movieDetail))
                        {
                            movieDetail = new MovieDetailsDto
                            {
                                Name = reader["Name"].ToString(),
                                ReleaseDate = (DateTime)reader["ReleaseDate"],
                                AgeRating = reader["AgeRating"].ToString(),
                                Actors = new List<ActorDetailsDto>()
                            };
                            movieDict.Add(movieId, movieDetail);
                        }

                        if (reader["ActorName"] != DBNull.Value)
                            movieDetail.Actors.Add(new ActorDetailsDto
                            {
                                Name = reader["ActorName"].ToString(),
                                Surname = reader["Surname"].ToString(),
                                CharacterName = reader["CharacterName"].ToString()
                            });
                    }
                }
            }
        }

        movies = movieDict.Values.ToList();
        return movies;
    }


    public async Task<bool> AssignActorToMovieAsync(int movieId, int actorId, string characterName)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var commandText = @"
            INSERT INTO Actor_Movie (IdMovie, IdActor, CharacterName) 
            VALUES (@IdMovie, @IdActor, @CharacterName)";

            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@IdMovie", movieId);
                command.Parameters.AddWithValue("@IdActor", actorId);
                command.Parameters.AddWithValue("@CharacterName", characterName);

                var result = await command.ExecuteNonQueryAsync();
                return result > 0;
            }
        }
    }
}