namespace WebApplication1.DTOs;

public class ActorDetailsDto
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string CharacterName { get; set; }
}

public class MovieDetailsDto
{
    public string Name { get; set; }
    public DateTime ReleaseDate { get; set; } 
    public string AgeRating { get; set; }
    public List<ActorDetailsDto> Actors { get; set; }
}

public class AssignActorDto
{
    public int MovieId { get; set; }
    public int ActorId { get; set; }
    public string CharacterName { get; set; }
}