using System.ComponentModel.DataAnnotations;
using Common.Enums;

namespace WebApi.Filters;

public class FilmFilterModel
{
    /// <summary>
    /// ������ id ����� ����������� �������. �������� countries=1,2,3. �� ������ ������ ����� ������� �� ����� ����� ������.
    /// </summary>
    public int[]? Countries { get; set; }
    /// <summary>
    /// ������ id ������ ����������� �������. �������� genres=1,2,3. �� ������ ������ ����� ������� �� ����� ������ �����.
    /// </summary>
    public int[]? Genres { get; set; }
    /// <summary>
    /// ����� ������ �� ������������
    /// </summary>
    public string? SearchQuery { get; set; }
    /// <summary>
    /// ����������
    /// </summary>
    public Order Order { get; set; }
}