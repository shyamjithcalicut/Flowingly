namespace Flowingly.Domain.Models;

/// <summary>
/// The processed expense detail.
/// </summary>

public class ProcessedExpenseDetail
{
    /// <summary>
    /// Gets or sets the cost centre of the expense.
    /// </summary>
    public string? CostCentre { get; set; }

    /// <summary>
    /// Gets or sets the total amount of the expense including tax.
    /// </summary>
    public decimal? TotalIncludingTax { get; set; }

    /// <summary>
    /// Gets or sets the total amount of the expense excluding tax.
    /// </summary>
    public decimal? TotalExcludingTax { get; set; }

    /// <summary>
    /// Gets or sets the tax amount of the expense.
    /// </summary>
    public decimal? Tax { get; set; }

    /// <summary>
    /// Gets or sets the payment method used for the expense.
    /// </summary>
    public string? PaymentMethod { get; set; }
}