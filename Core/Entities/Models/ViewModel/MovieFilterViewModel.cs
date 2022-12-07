using Core.Entities.Enums;

namespace Core.Entities.Models.ViewModel;

public class MovieFilterViewModel
{
    /// <summary>
    /// список  стран разделенные зап€той. Ќапример =1,2,3. Ќа данный момент можно указать не более одной страны.
    /// </summary>
    public int[]? Countries { get; set; }
    /// <summary>
    /// список  жанров разделенные зап€той. Ќапример =1,2,3. Ќа данный момент можно указать не более одного жанра.
    /// </summary>
    public int[]? Genres { get; set; }
    /// <summary>
    /// поиск фильма по наименованию
    /// </summary>
    public string? SearchQuery { get; set; }

    /// <summary>
    /// сортировка
    /// </summary>
    public MovieSortState SortOrder { get; set; } = MovieSortState.NameAsc;

    public int PageSize { get; set; } = 10;

    public int Page { get; set; }
}