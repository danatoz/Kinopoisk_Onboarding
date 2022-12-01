using Common.Enums;
using Entities;

namespace WebApi.Models;

public class MovieModel
{
    
    public int kinopoiskId { get; set; }
    
    public string? nameRu { get; set; }
    
    public string? posterUrl { get; set; }
    
    public List<CountryModel>? countries { get; set; }

    public List<GenreModel>? genres { get; set; } 

    public string? duration { get; set; }

    public static Movie? ConvertToEntity(MovieModel? obj)
    {
        double.TryParse(obj?.duration, out double duration);
        return obj != null ? new Movie
        {
            KinopoiskId = obj.kinopoiskId,
            NameRu = obj.nameRu,
            PosterUrl = obj.posterUrl,
            Duration = TimeSpan.FromMinutes(duration),
            Genres = GenreModel.GenreLineToGenreEnums(obj.genres).Sum(item => (int)item),
            Countries = CountryModel.ConvertToEntities(obj.countries)
        } : null;
    }

    public static List<Movie?> ConvertToEntities(List<MovieModel>? objs)
    {
        return objs.Select(ConvertToEntity).ToList();
    }

    public static MovieModel? ConvertToModel(Movie? entity)
    {
        
        return entity != null ? new MovieModel
        {
            kinopoiskId = entity.KinopoiskId,
            nameRu = entity.NameRu,
            posterUrl = entity.PosterUrl,
            duration = entity.Duration?.TotalMinutes.ToString(),
            genres = GenreModel.CovertEnumToModel(entity.Genres),
            countries = CountryModel.ConvertToModels(entity.Countries)
        } : null;
    }

    public static List<MovieModel?> ConvertToModels(List<Movie>? entities)
    {
        return entities.Select(ConvertToModel).ToList();
    }
}