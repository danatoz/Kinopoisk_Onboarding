namespace Core.Configurations;

public class HttpRequestConfiguration
{
    public string Uri { get; set; }

    public HttpMethod Method { get; set; }

    public Dictionary<string, string> Headers { get; set; }
}
