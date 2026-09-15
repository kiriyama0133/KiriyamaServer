using KiriyamaServer.Domain;

namespace KiriyamaServer.Application.Exceptions;

public sealed class InputValidationException(string message) : DomainException(message);
