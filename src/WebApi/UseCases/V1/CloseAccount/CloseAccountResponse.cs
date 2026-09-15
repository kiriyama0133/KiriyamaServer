using KiriyamaServer.Application.Boundaries.CloseAccount;
using System.ComponentModel.DataAnnotations;

namespace KiriyamaServer.WebApi.UseCases.V1.CloseAccount;

/// <summary>
/// CloseAsync Account Response.
/// </summary>
public sealed class CloseAccountResponse
{
    /// <summary>
    /// Account ID.
    /// </summary>
    [Required]
    public Guid AccountId { get; }

    public CloseAccountResponse(CloseAccountOutput output)
    {
        AccountId = output.AccountId;
    }
}