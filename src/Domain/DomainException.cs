namespace KiriyamaServer.Domain;

public class DomainException(string businessMessage) : Exception(businessMessage);
