namespace WebApplication1.Models;

public class Actor_Movie
{
    public int IdActorMovie { get; set; }
    public int IdMovie { get; set; }
    public int IdActor { get; set; }
    public string CharacterName { get; set; }
    public Actor Actor { get; set; }
}