namespace WebApi.Filters;

public class FilmFilterModel
{
    /// <summary>
    /// список id стран разделенные зап€той. Ќапример countries=1,2,3. Ќа данный момент можно указать не более одной страны.
    /// </summary>
    public int[]? Countries { get; set; }
    /// <summary>
    /// список id жанров разделенные зап€той. Ќапример genres=1,2,3. Ќа данный момент можно указать не более одного жанра.
    /// </summary>
    public int[]? Genres { get; set; }
    /// <summary>
    /// поиск фильма по наименованию
    /// </summary>
    public string? SearchQuery { get; set; }
}