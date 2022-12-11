using Core.Configurations;

namespace BL.Abstract;

public interface IApiService<T>
{
    Task<ApiResponseWrapper<T>> Request(HttpRequestConfiguration cfg);
}