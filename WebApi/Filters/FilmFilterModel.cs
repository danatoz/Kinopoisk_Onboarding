namespace WebApi.Filters;

public class FilmFilterModel
{
    /// <summary>
    /// список id стран разделенные запятой. Например countries=1,2,3. На данный момент можно указать не более одной страны.
    /// </summary>
    public int[]? Countries { get; set; }
    /// <summary>
    /// список id жанров разделенные запятой. Например genres=1,2,3. На данный момент можно указать не более одного жанра.
    /// </summary>
    public int[]? Genres { get; set; }
    /// <summary>
    /// Страница
    /// </summary>
    public int? Page { get; set; }
}