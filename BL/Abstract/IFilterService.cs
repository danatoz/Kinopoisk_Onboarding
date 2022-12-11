using Core.Utilities.Results;

namespace BL.Abstract;

public interface IFilterService
{
    FilterWrapper GetAll();

    Task<int> Download(CancellationToken cancel);
}