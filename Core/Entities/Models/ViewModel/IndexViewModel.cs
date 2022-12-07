namespace Core.Entities.Models.ViewModel;

public class IndexViewModel<T>
{
    public List<MovieModel?>? Items { get; set; }
    public PageViewModel? PageViewModel { get; set; }
}