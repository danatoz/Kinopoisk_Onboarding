using System.Net;

namespace BL;

public class ApiResponseWrapper<T>
{
    public HttpStatusCode StatusCode { get; set; }

    public T Data { get; set; }
}