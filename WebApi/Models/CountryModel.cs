using Entities;

namespace WebApi.Models;

public class CountryModel
{
    public string country { get; set; }

    public static Country? ConvertToEntity(CountryModel? obj)
    {
        return obj != null ? new Country
        {
            Name = obj.country
        } : null;
    }

    public static List<Country> ConvertToEntities(List<CountryModel> objs)
    {
        return objs.Select(ConvertToEntity).ToList();
    }
}