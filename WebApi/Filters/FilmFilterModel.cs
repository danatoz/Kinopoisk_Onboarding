using System.ComponentModel.DataAnnotations;
using Common.Enums;

namespace WebApi.Filters;

public class FilmFilterModel
{
    /// <summary>
    /// список  стран разделенные запятой. Например =1,2,3. На данный момент можно указать не более одной страны.
    /// </summary>
    public int[]? Countries { get; set; }
    /// <summary>
    /// список  жанров разделенные запятой. Например =1,2,3. На данный момент можно указать не более одного жанра.
    /// </summary>
    public int[]? Genres { get; set; }
    /// <summary>
    /// поиск фильма по наименованию
    /// </summary>
    public string? SearchQuery { get; set; }
    /// <summary>
    /// сортировка
    /// </summary>
    public Order Order { get; set; }
}