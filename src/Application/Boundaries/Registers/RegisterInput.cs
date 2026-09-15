using KiriyamaServer.Application.Exceptions;
using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Domain.ValueObjects;

namespace KiriyamaServer.Application.Boundaries.Registers;

public sealed class RegisterInput(SSN? ssn, Name? name, PositiveMoney? initialAmount) : IInputType
{
    public SSN SSN { get; } = ssn ?? throw new InputValidationException($"{nameof(ssn)} cannot be null.");
    public Name Name { get; } = name ?? throw new InputValidationException($"{nameof(name)} cannot be null.");
    public PositiveMoney InitialAmount { get; } = initialAmount ?? throw new InputValidationException($"{nameof(initialAmount)} cannot be null.");
}