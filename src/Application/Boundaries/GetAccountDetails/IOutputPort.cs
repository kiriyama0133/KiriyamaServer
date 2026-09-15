using KiriyamaServer.Application.Interfaces;

namespace KiriyamaServer.Application.Boundaries.GetAccountDetails;

public interface IOutputPort : IOutputPort<GetAccountDetailsOutput>
{
    void NotFound(string message);
}