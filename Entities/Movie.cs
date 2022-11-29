using Common.Enums;
namespace Entities;

class Movie
{
    
    public int KinopoiskId { get; set; }
    
    public string? NameRu { get; set; }
    
    public string? PosterUrl { get; set; }
    
    public IEnumerable<Country>? Countries { get; set; }
    
    public IEnumerable<Genre>? Genres { get; set; } 
    
    public TimeSpan Duration { get; set; }
}