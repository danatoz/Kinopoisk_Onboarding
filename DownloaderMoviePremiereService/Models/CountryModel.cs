using Entities;

namespace DownloaderMoviePremiereService.Models;

public class CountryModel
{
    public string? country { get; set; }

    public static Country? ConvertToEntity(CountryModel? obj)
    {
        return obj != null ? new Country
        {
            Name = obj.country
        } : null;
    }

    public static List<Country?>? ConvertToEntities(List<CountryModel>? objs)
    {
        return objs != null ? objs.Select(ConvertToEntity).ToList()! : new List<Country>();
    }

    public static CountryModel? ConvertToModel(Country? entity)
    {
        return entity != null
            ? new CountryModel
            {
                country = entity?.Name
            }
            : null;
    }

    public static List<CountryModel> ConvertToModels(List<Country?>? entities)
    {
        return entities != null ? entities.Select(ConvertToModel).ToList()! : new List<CountryModel>();
    }
}