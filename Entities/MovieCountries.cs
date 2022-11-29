using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities;

[Keyless]
public class MovieCountries
{
    public int KinopoiskId { get; set; }

    public int CountryId { get; set; }

    [ForeignKey(nameof(CountryId))]
    public Country? Country { get; set; }

    [ForeignKey(nameof(KinopoiskId))]
    public Movie? Movie { get; set; }
}