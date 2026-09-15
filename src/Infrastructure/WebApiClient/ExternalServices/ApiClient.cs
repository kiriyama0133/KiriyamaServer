using KiriyamaServer.Application.Services;
using Newtonsoft.Json;

namespace KiriyamaServer.Infrastructure.WebApiClient.ExternalServices;

public abstract class ApiClient(HttpClient httpClient) : IApiClient
{
    protected readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    protected static HttpContent PackageContent<T>(T transactionRequest)
        where T : class
    {
        StringContent content = new StringContent(JsonConvert.SerializeObject(transactionRequest));
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        return content;
    }
}
