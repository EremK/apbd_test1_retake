using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOs;
using WebApplication1.Repositories;

namespace WebApplication1.Controllers;

[ApiController]
[Route("[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMovieRepository _movieRepository;

    public MoviesController(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetMovies([FromQuery] DateTime? releaseDate)
    {
        var movies = await _movieRepository.GetAllMoviesAsync(releaseDate);
        return Ok(movies);
    }

    [HttpPost("assignActor")]
    public async Task<IActionResult> AssignActorToMovie([FromBody] AssignActorDto dto)
    {
        var result = await _movieRepository.AssignActorToMovieAsync(dto.MovieId, dto.ActorId, dto.CharacterName);
        if (!result) return BadRequest("Failed to assign actor to movie.");
        return Ok("Actor assigned successfully.");
    }
}