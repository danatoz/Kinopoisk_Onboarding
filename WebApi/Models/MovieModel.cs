using System.Globalization;
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

    public static Movie? ConvertToEntity(MovieModel? model)
    {
        double.TryParse(model?.duration, out double duration);
        return model != null ? new Movie
        {
            KinopoiskId = model.kinopoiskId,
            NameRu = model.nameRu,
            PosterUrl = model.posterUrl,
            Duration = TimeSpan.FromMinutes(duration),
            Genres = GenreModel.GenreLineToGenreEnums(model.genres).Sum(item => (int)item),
            Countries = CountryModel.ConvertToEntities(model.countries)
        } : null;
    }

    public static List<Movie?> ConvertToEntities(List<MovieModel>? models)
    {
        return models != null ? models.Select(ConvertToEntity).ToList() : new List<Movie?>();
    }

    public static MovieModel? ConvertToModel(Movie? entity)
    {
        
        return entity != null ? new MovieModel
        {
            kinopoiskId = entity.KinopoiskId,
            nameRu = entity.NameRu,
            posterUrl = entity.PosterUrl,
            duration = entity.Duration?.TotalMinutes.ToString(CultureInfo.InvariantCulture),
            genres = GenreModel.CovertEnumToModel(entity.Genres),
            countries = CountryModel.ConvertToModels(entity.Countries)
        } : null;
    }

    public static List<MovieModel?> ConvertToModels(List<Movie>? entities)
    {
        return entities != null ? entities.Select(ConvertToModel).ToList() : new List<MovieModel?>();
    }
}