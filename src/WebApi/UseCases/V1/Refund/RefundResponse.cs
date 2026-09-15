using System.ComponentModel.DataAnnotations;

namespace KiriyamaServer.WebApi.UseCases.V1.Refund;

public sealed class RefundResponse(decimal amount, string description, DateTime transactionDate, decimal updatedBalance)
{
    [Required]
    public decimal Amount { get; } = amount;

    [Required]
    public string Description { get; } = description;

    [Required]
    public DateTime TransactionDate { get; } = transactionDate;

    [Required]
    public decimal UpdateBalance { get; } = updatedBalance;
}