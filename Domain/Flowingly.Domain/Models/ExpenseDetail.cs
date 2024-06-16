namespace Flowingly.Domain.Models;

/// <summary>
/// Represents an expense detail.
/// </summary>
public class ExpenseDetail
{
    /// <summary>
    /// Gets or sets the cost centre of the expense.
    /// </summary>
    public string? CostCentre { get; set; }

    /// <summary>
    /// Gets or sets the total amount of the expense.
    /// </summary>
    public decimal? Total { get; set; }

    /// <summary>
    /// Gets or sets the payment method used for the expense.
    /// </summary>
    public string? PaymentMethod { get; set; }
}