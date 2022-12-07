using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Enums;

[Flags]
public enum Genre
{
    [Display(Name="триллер")]
    Thriller = 1,

    [Display(Name="драма")]
    Drama = 2,

    [Display(Name="криминал")]
    Crime = 4,

    [Display(Name="мелодрама")]
    Melodrama = 8,

    [Display(Name="детектив")]
    Detective = 16,

    [Display(Name="фантастика")]
    Fiction = 32,

    [Display(Name="приключения")]
    Adventure = 64,

    [Display(Name="биография")]
    Biography = 128,

    [Display(Name="фильм-нуар")]
    Filmnoir = 256,

    [Display(Name="вестерн")]
    Western = 512,

    [Display(Name = "боевик")]
    Action = 1024,

    [Display(Name = "фэнтези")]
    Fantasy = 2048,

    [Display(Name = "комедия")]
    Comedy = 4096,
}