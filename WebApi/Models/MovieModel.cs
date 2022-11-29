using Common.Enums;

namespace WebApi.Models;

class MovieModel
{
    
    public int kinopoiskId { get; set; }
    
    public string nameRu { get; set; }
    
    public string posterUrl { get; set; }
    
    public List<CountryModel>? countries { get; set; }
    
    public List<Genre>? genres { get; set; } 
    
    public string duration { get; set; }
}