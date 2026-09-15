using System.ComponentModel.DataAnnotations;

namespace KiriyamaServer.WebApi.UseCases.V1.GetAccountDetails;

/// <summary>
/// The GetAsync Account Details Request.
/// </summary>
public sealed class GetAccountDetailsRequest
{
    /// <summary>
    /// Account ID.
    /// </summary>
    [Required]
    public Guid AccountId { get; set; }
}