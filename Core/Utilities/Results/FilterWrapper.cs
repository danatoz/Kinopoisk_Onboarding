using Core.Entities.Concrete;
using Core.Entities.Models;

namespace Core.Utilities.Results;

public class FilterWrapper
{
    public List<GenreModel> Genres { get; set; }

    public List<CountryModel> Countries { get; set; }
}