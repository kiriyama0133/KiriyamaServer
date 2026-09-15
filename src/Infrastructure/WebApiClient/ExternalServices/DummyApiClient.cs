using KiriyamaServer.Application.Services;
using KiriyamaServer.Infrastructure.WebApiClient.Exceptions;
using KiriyamaServer.Contracts.ReadModels;

namespace KiriyamaServer.Infrastructure.WebApiClient.ExternalServices;

public class DummyApiClient : ApiClient, IDummyApiClient
{
    public DummyApiClient(HttpClient httpClient)
        : base(httpClient)
    {
    }

    public async Task<SimpleResult> GetSimpleModelAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/Dummy/{id}", cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<SimpleResult>(cancellationToken);
            }

            throw new BackendServiceCallFailedException(response.ReasonPhrase);
        }
        catch (BackendServiceCallFailedException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new BackendServiceCallFailedException(e.Message, e);
        }
    }
}
