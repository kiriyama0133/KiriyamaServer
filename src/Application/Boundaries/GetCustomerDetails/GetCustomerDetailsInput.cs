using KiriyamaServer.Application.Exceptions;
using KiriyamaServer.Application.Interfaces;

namespace KiriyamaServer.Application.Boundaries.GetCustomerDetails;

public sealed class GetCustomerDetailsInput : IInputType
{
    public Guid CustomerId { get; }

    public GetCustomerDetailsInput(Guid customerId)
    {
        if (customerId == Guid.Empty)
        {
            throw new InputValidationException($"{nameof(customerId)} cannot be empty.");
        }

        CustomerId = customerId;
    }
}