using System.ComponentModel.DataAnnotations;
using Common.Enums;
using Entities;

namespace WebApi.Models;

public class GenreModel
{
    public string? genre { get; set; }

    public static Genre GenreLineToGenreEnum(GenreModel model)
    {
        var aGenres = Enum.GetValues(typeof(Genre)).Cast<Genre>()
            .Select(item => new { Name = item ,DisplayName = item.GetAttribute<DisplayAttribute>()?.Name });

        var result = aGenres.FirstOrDefault(item => item.DisplayName == model.genre)?.Name;

        return result ?? Genre.Thriller;
    }

    public static List<Genre> GenreLineToGenreEnums(List<GenreModel>? models)
    {
        return models.Select(GenreLineToGenreEnum).ToList();
    }

    public static List<GenreModel> CovertEnumToModel(int genreInt)
    {
        var genreArrLine = ((Genre)genreInt).ToString().Replace(",", "").Split();
        var result = new List<GenreModel>();
        foreach (var genreLine in genreArrLine)
        {
            Enum.TryParse(genreLine, out Genre genre);
            result.Add(new GenreModel
            {
                genre = genre.GetAttribute<DisplayAttribute>()?.Name
            });
        }

        return result;
    }
}