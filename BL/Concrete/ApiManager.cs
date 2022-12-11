using BL.Abstract;
using Core.Configurations;
using Newtonsoft.Json;

namespace BL.Concrete;

public class ApiManager<T> : IApiService<T>
{
    private readonly HttpClient _httpClient = new HttpClient();

    public async Task<ApiResponseWrapper<T>> Request(HttpRequestConfiguration cfg)
    {
        var result = new ApiResponseWrapper<T>();
        using var request = new HttpRequestMessage(cfg.Method, cfg.Uri);
        foreach (var item in cfg.Headers)
        {
            request.Headers.Add(item.Key, item.Value);
        }
        using var response = await _httpClient.SendAsync(request);

        result.StatusCode = response.StatusCode;

        var content = await response.Content.ReadAsStringAsync();
        result.Data = JsonConvert.DeserializeObject<T>(content);

        return result;
    }
}