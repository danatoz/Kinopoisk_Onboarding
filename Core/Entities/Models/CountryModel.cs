using AutoMapper;
using Core.Entities.Concrete;

namespace Core.Entities.Models;

public class CountryModel
{
    public int Id { get; set; }

    public string? country { get; set; }

    public static Country? ConvertToEntity(CountryModel? obj)
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CountryModel, Country>()
                .ForMember(dest =>
                    dest.Name, act => 
                    act.MapFrom(src => src.country))
                .ForMember(dest => dest.Id, act => act.Ignore());
        });
        var mapper = config.CreateMapper();
        var result = mapper.Map<Country>(obj);
        return result;
    }

    public static List<Country?> ConvertToEntities(List<CountryModel>? objs)
    {
        return objs != null ? objs.Select(ConvertToEntity).ToList() : new List<Country?>();
    }

    public static CountryModel? ConvertToModel(Country? entity)
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Country, CountryModel>()
                .ForMember(dest => 
                    dest.country, act => 
                    act.MapFrom(src => src.Name));
        });
        var mapper = config.CreateMapper();
        return mapper.Map<CountryModel>(entity);
    }

    public static List<CountryModel?> ConvertToModels(List<Country?>? entities)
    {
        return entities != null ? entities.Select(ConvertToModel).ToList() : new List<CountryModel?>();
    }
}