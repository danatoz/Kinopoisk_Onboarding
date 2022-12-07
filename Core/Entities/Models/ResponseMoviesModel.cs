namespace Core.Entities.Models;

public class ResponseMoviesModel
{
    public int total { get; set; }

    public MovieModel[]? items { get; set; }
}