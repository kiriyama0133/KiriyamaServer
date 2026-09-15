using System.ComponentModel.DataAnnotations;

namespace KiriyamaServer.WebApi.UseCases.V1.GetCustomerDetails;

/// <summary>
/// The GetAsync Customer Details Request.
/// </summary>
public sealed class GetCustomerDetailsRequest
{
    /// <summary>
    /// Customer ID.
    /// </summary>
    [Required]
    public Guid CustomerId { get; set; }
}