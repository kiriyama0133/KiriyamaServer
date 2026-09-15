using KiriyamaServer.Application.Interfaces;

namespace KiriyamaServer.Application.Boundaries.GetCustomerDetails;

public interface IOutputPort : IOutputPort<GetCustomerDetailsOutput>
{
    void NotFound(string message);
}