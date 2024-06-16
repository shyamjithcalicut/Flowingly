namespace Flowingly.Domain.Models;

/// <summary>
/// Represents a reservation.
/// </summary>
public class Reservation
{
    /// <summary>
    /// Gets or sets the vendor of the reservation.
    /// </summary>
    public string? Vendor { get; set; }

    /// <summary>
    /// Gets or sets the description of the reservation.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the date of the reservation.
    /// </summary>
    public DateTime? Date { get; set; }

    /// <summary>
    /// Gets or sets the expense details of the reservation.
    /// </summary>
    public List<ExpenseDetail>? ExpenseDetails { get; set; }
}