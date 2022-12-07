using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Core.Entities.Concrete;

[Index(nameof(NameRu))]
[PrimaryKey(nameof(Id))]
public class Movie
{
    public int Id { get; set; }

    public int KinopoiskId { get; set; }

    public string? NameRu { get; set; }

    public string? PosterUrl { get; set; }

    public List<Country?>? Countries { get; set; }

    public int Genres { get; set; }

    public TimeSpan? Duration { get; set; }
}