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
    /// ��������
    /// </summary>
    public int? Page { get; set; }
}