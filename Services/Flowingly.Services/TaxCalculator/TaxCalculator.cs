using Flowingly.Contracts.TaxCalculator;

namespace Flowingly.Services.TaxCalculator;

/// <summary>
/// Represents a tax calculator.
/// </summary>
public class TaxCalculator : ITaxCalculator
{
   
    /// <summary>
    /// Calculates the tax amount from the total amount including tax.
    /// </summary>
    /// <param name="taxPercentage">The tax percentage.</param>
    /// <param name="totalAmountIncludingTax">The total amount including tax.</param>
    /// <returns>Tax Amount</returns>
    public decimal CalculateTaxFromTotalAmount(decimal taxPercentage, decimal totalAmountIncludingTax)
    {
        decimal taxAmount =  totalAmountIncludingTax/ taxPercentage;
        return taxAmount;
    }
}
