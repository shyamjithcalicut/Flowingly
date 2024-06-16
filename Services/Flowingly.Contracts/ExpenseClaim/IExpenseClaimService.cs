using Flowingly.Domain.Models;

namespace Flowingly.Contracts.ExpenseClaim;

/// <summary>
/// Represents the interface for the Expense Claim service.
/// </summary>
public interface IExpenseClaimService
{
    /// <summary>
    /// Processes the given text and returns the processed reservation.
    /// </summary>
    /// <param name="text">The text to be processed.</param>
    /// <returns>The processed reservation.</returns>
    ProcessedReservation ProcessText(string text);
}