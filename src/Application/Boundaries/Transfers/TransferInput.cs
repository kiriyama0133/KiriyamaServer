using KiriyamaServer.Application.Exceptions;
using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Domain.ValueObjects;

namespace KiriyamaServer.Application.Boundaries.Transfers;

public sealed class TransferInput : IInputType
{
    public Guid OriginAccountId { get; }
    public Guid DestinationAccountId { get; }
    public PositiveMoney Amount { get; }

    public TransferInput(Guid originAccountId, Guid destinationAccountId, PositiveMoney? amount)
    {
        if (originAccountId == Guid.Empty)
        {
            throw new InputValidationException($"{nameof(originAccountId)} cannot be empty.");
        }

        if (destinationAccountId == Guid.Empty)
        {
            throw new InputValidationException($"{nameof(destinationAccountId)} cannot be empty.");
        }

        OriginAccountId = originAccountId;
        DestinationAccountId = destinationAccountId;
        Amount = amount ?? throw new InputValidationException($"{nameof(amount)} cannot be null.");
    }
}