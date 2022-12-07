using Core.Configurations;
using Newtonsoft.Json;

namespace BL;

public class ApiRequestBL<T>
{
    private readonly HttpClient _httpClient = new HttpClient();

    public async Task<T?> Request(HttpRequestConfiguration cfg)
    {
        using var request = new HttpRequestMessage(cfg.Method, cfg.Uri);
        foreach (var item in cfg.Headers)
        {
            request.Headers.Add(item.Key, item.Value);   
        }
        using var response = await _httpClient.SendAsync(request);
        T? result = default;
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            result = JsonConvert.DeserializeObject<T>(content);
        }

        return result;
    }
}