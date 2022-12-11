using System.Globalization;
using AutoMapper;
using Core.Entities.Concrete;

namespace Core.Entities.Models;

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

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<MovieModel, Movie>()
                .ForMember(o => o.Duration, o =>
                    o.MapFrom(v => TimeSpan.FromMinutes(duration)))
                .ForMember(dest => dest.Countries, 
                    act => 
                        act.MapFrom(src => CountryModel.ConvertToEntities(src.countries)))
                .ForMember(dest => dest.Genres, 
                    act => act.MapFrom(src => 
                        GenreModel.GenreLineToGenreEnums(src.genres).Sum(item => (int)item)));
        });
        var mapper = config.CreateMapper();

        var result = mapper.Map<Movie>(model);
        return result;
    }

    public static List<Movie?> ConvertToEntities(List<MovieModel>? models)
    {
        return models != null ? models.Select(ConvertToEntity).ToList() : new List<Movie?>();
    }

    public static MovieModel? ConvertToModel(Movie? entity)
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Movie, MovieModel>()
                .ForMember(dest => dest.duration,
                    act =>
                    act.MapFrom(src => src.Duration.Value.TotalMinutes.ToString(CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.genres, 
                    act => 
                        act.MapFrom(src => GenreModel.CovertEnumToModel(src.Genres)))
                .ForMember(dest => dest.countries, 
                    act => 
                        act.MapFrom(src => CountryModel.ConvertToModels(src.Countries)));
        });
        var mapper = config.CreateMapper();

        return mapper.Map<MovieModel>(entity);
    }

    public static List<MovieModel?> ConvertToModels(List<Movie>? entities)
    {
        return entities != null ? entities.Select(ConvertToModel).ToList() : new List<MovieModel?>();
    }
}