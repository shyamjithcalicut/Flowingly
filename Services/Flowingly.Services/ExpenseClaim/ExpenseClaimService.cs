using Flowingly.Contracts.DataExtractor;
using Flowingly.Contracts.ExpenseClaim;
using Flowingly.Contracts.TaxCalculator;
using Flowingly.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Flowingly.Services.ExpenseClaim;

/// <summary>
/// Represents the Expense Claim service.
/// </summary>
public class ExpenseClaimService : IExpenseClaimService
{
    /// <summary>
    /// The default cost centre.
    /// </summary>
    private const string COST_CENTRE = "UNKNOWN";

    /// <summary>
    /// The default sales tax percentage.
    /// </summary>
    private const decimal DEFAULT_SALES_TAX_PERCENTAGE = 10;

    private readonly IReservationDataExtractor _dataExtractor;
    private readonly ITaxCalculator taxCalculator;
    private readonly ILogger<ExpenseClaimService> _logger;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpenseClaimService"/> class.
    /// </summary>
    /// <param name="dataExtractor">The data extractor.</param>
    /// <param name="taxCalculator">The tax calculator.</param>
    /// <param name="_logger">The logger.</param>
    /// <param name="configuration">The configuration.</param>
    public ExpenseClaimService(IReservationDataExtractor dataExtractor, ITaxCalculator taxCalculator, ILogger<ExpenseClaimService> _logger, IConfiguration configuration)
    {
        _dataExtractor = dataExtractor;
        this.taxCalculator = taxCalculator;
        this._logger = _logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Processes the text and returns the processed reservation.
    /// </summary>
    /// <param name="text">The text to process.</param>
    /// <returns>The processed reservation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the text is null or empty.</exception>
    public ProcessedReservation ProcessText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            _logger.LogError($"Could not process the request,The {nameof(text)} empty or null.");
            throw new ArgumentNullException(text, $"Could not process the request,The {nameof(text)} empty or null.");
        }

        var reservation = _dataExtractor.ExtractReservationDetails(text);

        if (reservation is null || reservation.ExpenseDetails is null || !reservation.ExpenseDetails.Any(x => (x.Total ?? 0) > 0))
        {
            _logger.LogError($"Could not extract data form the request, please verify the request.");
            throw new ArgumentNullException(text, $"Could not extract data form the request, please verify the request.");
        }

        _logger.LogInformation($"Successfully extracted the data from the request.");
        var salesTaxPercentage = _configuration.GetSection("SalesTaxPercentage") is not null ? (Convert.ToDecimal(_configuration.GetSection("SalesTaxPercentage").Value)) : DEFAULT_SALES_TAX_PERCENTAGE;
        ProcessedReservation processedReservation = new ProcessedReservation
        {
            Date = reservation.Date,
            Description = reservation.Description,
            Vendor = reservation.Vendor,
            ExpenseDetails = reservation.ExpenseDetails.Select(expenseDetail =>
            {
                var tax = taxCalculator.CalculateTaxFromTotalAmount(salesTaxPercentage, expenseDetail.Total ?? 0);
                return new ProcessedExpenseDetail
                {
                    TotalIncludingTax = expenseDetail.Total,
                    Tax = tax,
                    TotalExcludingTax = expenseDetail.Total - tax,
                    CostCentre = !string.IsNullOrEmpty(expenseDetail.CostCentre) ? expenseDetail.CostCentre : COST_CENTRE,
                    PaymentMethod = expenseDetail.PaymentMethod
                };
            }).ToList()
        };
        _logger.LogInformation($"Successfully processed the request.");

        return processedReservation;
    }
}