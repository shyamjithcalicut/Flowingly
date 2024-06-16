using Flowingly.Contracts.DataExtractor;
using Flowingly.Domain.Models;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Flowingly.Services.DataExtractor;

/// <summary>
/// Represents an XML data extractor for extracting reservation details.
/// </summary>
public class XmlReservationDataExtractor : IReservationDataExtractor
{
    #region Public Methods

    /// <summary>
    /// Extracts reservation details including its expense details from the given text based on XML format.
    /// </summary>
    /// <param name="text">The XML text containing reservation details.</param>
    /// <returns>The extracted Reservation object.</returns>
    public Reservation ExtractReservationDetails(string text)
    {
        var reservation = new Reservation();

        // Extract XML part
        var expensePattern = @"<expense>.*?<\/expense>";
        var expenseMatches = Regex.Matches(text, expensePattern, RegexOptions.Singleline);

        var expenseDetails = new List<ExpenseDetail>();
        foreach (Match match in expenseMatches)
        {
            if (!IsValidXml(match.Value))
            {
                throw new FormatException("Invalid XML format.");
            }
            var expenseDetail = ExtractExpenseDetails(match.Value);
            if (expenseDetail != null)
            {
                expenseDetails.Add(expenseDetail);
            }
        }

        reservation.ExpenseDetails = expenseDetails;

        // Extract remaining fields using regular expressions
        reservation.Vendor = ExtractField(text, "vendor");
        reservation.Description = ExtractField(text, "description");
        reservation.Date = ExtractDateField(text, "date");

        return reservation;
    }

    #endregion Public Methods

    #region Private Methods

    /// <summary>
    /// Extracts the expense details from the given XML.
    /// </summary>
    /// <param name="xml">The XML containing the expense details.</param>
    /// <returns>The extracted ExpenseDetail object.</returns>
    private ExpenseDetail ExtractExpenseDetails(string xml)
    {
        var doc = XDocument.Parse(xml);
        var expenseElement = doc.Root;

        var costCentre = expenseElement?.Element("cost_centre")?.Value;
        var totalString = expenseElement?.Element("total")?.Value?.Replace(",", string.Empty);
        var paymentMethod = expenseElement?.Element("payment_method")?.Value;

        decimal? total = null;
        if (decimal.TryParse(totalString, out var parsedTotal))
        {
            total = parsedTotal;
        }

        return new ExpenseDetail
        {
            CostCentre = costCentre,
            Total = total,
            PaymentMethod = paymentMethod
        };
    }

    /// <summary>
    /// Extracts the value of a specific field from the given input text based on the provided field name.
    /// </summary>
    /// <param name="inputText">The input text containing the field value.</param>
    /// <param name="fieldName">The name of the field to extract.</param>
    /// <returns>The extracted field value, or null if the field is not found.</returns>
    private string? ExtractField(string inputText, string fieldName)
    {
        var pattern = $@"<{fieldName}>(.*?)<\/{fieldName}>";
        var match = Regex.Match(inputText, pattern, RegexOptions.Singleline);
        if (!IsValidXml(match.Value))
        {
            throw new FormatException("Invalid XML format.");
        }
        return match.Success ? match.Groups[1].Value : null;
    }

    /// <summary>
    /// Extracts the value of a specific field from the given input text based on the provided field name.
    /// </summary>
    /// <param name="inputText">The input text containing the field value.</param>
    /// <param name="fieldName">The name of the field to extract.</param>
    /// <returns>The extracted field value, or null if the field is not found.</returns>
    private DateTime? ExtractDateField(string inputText, string fieldName)
    {
        var pattern = $@"<{fieldName}>(.*?)<\/{fieldName}>";
        var match = Regex.Match(inputText, pattern, RegexOptions.Singleline);
        if (!IsValidXml(match.Value))
        {
            throw new FormatException("Invalid XML format.");
        }
        if (match.Success && DateTime.TryParseExact(match.Groups[1].Value, "dd MMMM yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return date;
        }
        return null;
    }

    /// <summary>
    /// Validates whether the given XML is well-formed.
    /// </summary>
    /// <param name="xml">The XML to validate.</param>
    /// <returns>True if the XML is valid, otherwise false.</returns>
    private bool IsValidXml(string xml)
    {
        try
        {
            XDocument.Parse(xml);
            return true;
        }
        catch (Exception ex)
        {
            // Log the validation error
            Console.WriteLine($"XML validation failed: {ex.Message}");
            return false;
        }
    }

    #endregion Private Methods
}