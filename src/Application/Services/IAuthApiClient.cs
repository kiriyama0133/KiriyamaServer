using KiriyamaServer.Contracts.ReadModels;

namespace KiriyamaServer.Application.Services;

public interface IAuthApiClient : IApiClient
{
    Task<SimpleResult> GetSimpleAuthModelAsync(string id, CancellationToken cancellationToken = default);
}
