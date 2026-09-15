using System.ComponentModel.DataAnnotations;

namespace KiriyamaServer.WebApi.UseCases.V2.GetAccountDetails;

/// <summary>
/// The GetAsync Account Details Request.
/// </summary>
public sealed class GetAccountDetailsRequestV2
{
    /// <summary>
    /// Account ID.
    /// </summary>
    [Required]
    public Guid AccountId { get; set; }
}