namespace Flowingly.Contracts.TaxCalculator;

/// <summary>
/// Represents a tax calculator.
/// </summary>
public interface ITaxCalculator
{
    /// <summary>
    /// Calculates the tax from the total amount (excluding tax), given the tax percentage and total amount (excluding tax).
    /// </summary>
    /// <param name="taxPercentage">The tax percentage.</param>
    /// <param name="totalAmountIncludingTax">The total amount (excluding tax).</param>
    /// <returns>The calculated tax amount.</returns>
    decimal CalculateTaxFromTotalAmount(decimal taxPercentage, decimal totalAmountIncludingTax);
}