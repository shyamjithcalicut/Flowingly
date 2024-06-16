using Flowingly.Domain.Models;

namespace Flowingly.Contracts.DataExtractor;

/// <summary>
/// The data extractor.
/// </summary>

public interface IReservationDataExtractor
{
    /// <summary>
    /// Extracts reservation details including its expense details from the given text based on various data formats like XML, JSON, etc.
    /// </summary>
    /// <param name="text">The text containing the reservation details.</param>
    /// <returns>The extracted Reservation object.</returns>
    Reservation ExtractReservationDetails(string text);
}