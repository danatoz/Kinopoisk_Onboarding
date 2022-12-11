using Core.Entities.Models.ViewModel;
using Core.Utilities.Results;

namespace BL.Abstract;

public interface IMovieService
{
    Task<MovieViewModel> GetPremieres(MovieFilterViewModel filters);

    Task<int> Download(CancellationToken cancel);
}