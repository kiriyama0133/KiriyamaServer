using KiriyamaServer.Contracts.ReadModels;

namespace KiriyamaServer.Application.Services;

public interface IDummyApiClient : IApiClient
{
    Task<SimpleResult> GetSimpleModelAsync(string id, CancellationToken cancellationToken = default);
}
