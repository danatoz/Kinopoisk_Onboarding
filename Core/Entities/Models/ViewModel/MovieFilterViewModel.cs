using Core.Entities.Enums;

namespace Core.Entities.Models.ViewModel;

public class MovieFilterViewModel
{
    /// <summary>
    /// ������  ����� ����������� �������. �������� =1,2,3. �� ������ ������ ����� ������� �� ����� ����� ������.
    /// </summary>
    public int[]? Countries { get; set; }
    /// <summary>
    /// ������  ������ ����������� �������. �������� =1,2,3. �� ������ ������ ����� ������� �� ����� ������ �����.
    /// </summary>
    public int[]? Genres { get; set; }
    /// <summary>
    /// ����� ������ �� ������������
    /// </summary>
    public string? SearchQuery { get; set; }

    /// <summary>
    /// ����������
    /// </summary>
    public MovieSortState SortOrder { get; set; } = MovieSortState.NameAsc;

    public int PageSize { get; set; } = 10;

    public int Page { get; set; }
}