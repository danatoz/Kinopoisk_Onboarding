using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Enums;
using Microsoft.EntityFrameworkCore;

namespace Entities;

[Index(nameof(NameRu))]
[PrimaryKey(nameof(KinopoiskId))]
public class Movie
{
    public int KinopoiskId { get; set; }
    
    public string? NameRu { get; set; }
    
    public string? PosterUrl { get; set; }
    
    public IEnumerable<Country>? Countries { get; set; }
    
    public Genre Genres { get; set; } 
    
    public TimeSpan Duration { get; set; }

    public int MoviePremiereUpdateLogId { get; set; }

    [ForeignKey(nameof(MoviePremiereUpdateLogId))]
    public MoviePremiereUpdateLog MoviePremiereUpdateLog { get; set; }
}