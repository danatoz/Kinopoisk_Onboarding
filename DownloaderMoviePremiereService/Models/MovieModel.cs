using Entities;

namespace DownloaderMoviePremiereService.Models;

class MovieModel
{
    public int kinopoiskId { get; set; }
    
    public string? nameRu { get; set; }
    
    public string? posterUrl { get; set; }
    
    public List<CountryModel>? countries { get; set; }

    public List<GenreModel>? genres { get; set; } 

    public string? duration { get; set; }

    public static Movie? ConvertToEntity(MovieModel? obj)
    {
        return obj != null ? new Movie
        {
            KinopoiskId = obj.kinopoiskId,
            NameRu = obj.nameRu,
            PosterUrl = obj.posterUrl,
            Duration = TimeSpan.Parse(obj.duration),
            //Genres = obj.genres,
            Countries = obj.countries! as IEnumerable<Country>
        } : null;
    }

    public static List<Movie?> ConvertToEntities(List<MovieModel>? objs)
    {
        return objs.Select(ConvertToEntity).ToList();
    }
}