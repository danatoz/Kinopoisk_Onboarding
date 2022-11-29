using System.ComponentModel.DataAnnotations;

namespace Common.Enums;

[Flags]
public enum Genre
{
    [Display(Name="триллер")]
    Thriller = 1,

    [Display(Name="драма")]
    Drama = 2,

    [Display(Name="криминал")]
    Crime = 3,

    [Display(Name="мелодрама")]
    Melodrama = 4,

    [Display(Name="детектив")]
    Detective = 5,

    [Display(Name="фантастика")]
    Fiction = 6,

    [Display(Name="приключения")]
    Adventure = 7,

    [Display(Name="биография")]
    Biography = 8,

    [Display(Name="фильм-нуар")]
    Filmnoir = 9,

    [Display(Name="вестерн")]
    Western = 10,
}