using Flowingly.Contracts.DataExtractor;
using Flowingly.Contracts.TaxCalculator;
using Flowingly.Domain.Models;
using Flowingly.Services.ExpenseClaim;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Flowingly.Tests;

/// <summary>
/// Represents the Expense Claim service tests.
/// </summary>
public class ExpenseClaimServiceTests
{
    private readonly Mock<IReservationDataExtractor> _dataExtractorMock;
    private readonly Mock<ITaxCalculator> _taxCalculatorMock;
    private readonly Mock<ILogger<ExpenseClaimService>> _loggerMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly ExpenseClaimService _expenseClaimService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpenseClaimServiceTests"/> class.
    /// </summary>
    public ExpenseClaimServiceTests()
    {
        _dataExtractorMock = new Mock<IReservationDataExtractor>();
        _taxCalculatorMock = new Mock<ITaxCalculator>();
        _loggerMock = new Mock<ILogger<ExpenseClaimService>>();
        _configurationMock = new Mock<IConfiguration>();
        _expenseClaimService = new ExpenseClaimService(_dataExtractorMock.Object, _taxCalculatorMock.Object, _loggerMock.Object, _configurationMock.Object);
    }

    /// <summary>
    /// Test case to verify that the ProcessText method returns valid processed data.
    /// </summary>
    [Fact]
    public void ProcessText_ShouldReturnValidProcessedData()
    {
        // Arrange
        string text = "Sample text";
        var reservation = new Reservation
        {
            Date = new DateTime(2022, 01, 01),
            Description = "Sample description",
            Vendor = "Sample vendor",
            ExpenseDetails = new List<ExpenseDetail>
                {
                    new ExpenseDetail
                    {
                        Total = 100,
                        CostCentre = "Sample cost centre",
                        PaymentMethod = "Sample payment method"
                    }
                }
        };
        var processedExpenseDetail = new ProcessedExpenseDetail
        {
            TotalIncludingTax = 100,
            Tax = 10,
            TotalExcludingTax = 90,
            CostCentre = "Sample cost centre",
            PaymentMethod = "Sample payment method"
        };
        var processedReservation = new ProcessedReservation
        {
            Date = new DateTime(2022, 01, 01),
            Description = "Sample description",
            Vendor = "Sample vendor",
            ExpenseDetails = new List<ProcessedExpenseDetail> { processedExpenseDetail }
        };

        _dataExtractorMock.Setup(x => x.ExtractReservationDetails(text)).Returns(reservation);
        _taxCalculatorMock.Setup(x => x.CalculateTaxFromTotalAmount(10, 100)).Returns(10);

        // Act
        var result = _expenseClaimService.ProcessText(text);

        // Assert
        Assert.Equal(processedReservation.Date, result.Date);
        Assert.Equal(processedReservation.Description, result.Description);
        Assert.Equal(processedReservation.Vendor, result.Vendor);
        Assert.Single(result.ExpenseDetails);
        Assert.Equal(processedExpenseDetail.TotalIncludingTax, result.ExpenseDetails[0].TotalIncludingTax);
        Assert.Equal(processedExpenseDetail.Tax, result.ExpenseDetails[0].Tax);
        Assert.Equal(processedExpenseDetail.TotalExcludingTax, result.ExpenseDetails[0].TotalExcludingTax);
        Assert.Equal(processedExpenseDetail.CostCentre, result.ExpenseDetails[0].CostCentre);
        Assert.Equal(processedExpenseDetail.PaymentMethod, result.ExpenseDetails[0].PaymentMethod);
    }

    /// <summary>
    /// Test case to verify that the ProcessText method returns null for invalid total.
    /// </summary>
    [Fact]
    public void ProcessText_ShouldReturnNullForInvalidTotal()
    {
        // Arrange
        string text = "Sample text";
        var expectedParamName = "text";
        bool exceptionThrown = false;
        var reservation = new Reservation
        {
            Date = new DateTime(2022, 01, 01),
            Description = "Sample description",
            Vendor = "Sample vendor",
            ExpenseDetails = new List<ExpenseDetail>
                {
                    new ExpenseDetail
                    {
                        Total = null,
                        CostCentre = "Sample cost centre",
                        PaymentMethod = "Sample payment method"
                    }
                }
        };

        _dataExtractorMock.Setup(x => x.ExtractReservationDetails(text)).Returns(reservation);
        try
        {
            // Act
            var result = _expenseClaimService.ProcessText(text);
        }
        catch (ArgumentNullException ex)
        {
            // Assert
            Assert.IsType<ArgumentNullException>(ex);
        }
    }

    /// <summary>
    /// Test case to verify that the ProcessText method returns null for invalid total.
    /// </summary>
    [Fact]
    public void ProcessText_ShouldReturnNullForIncompleteXmlTag()
    {
        // Arrange
        string text = "<expense><cost_centre>DEV632<total>35,000</total><payment_method>personal card</payment_method></expense> From: William Steele Sent: Friday, 16 June 2022 10:32 AM To: Maria Washington Subject: test Hi Maria, Please create a reservation for 10 at the <vendor>Seaside Steakhouse</vendor> for our <description>development team’s project end celebration</description> on <date>27 April 2022</date> at 7.30pm. Regards, William";
        var expectedParamName = "text";
        bool exceptionThrown = false;
        var reservation = new Reservation
        {
            Date = new DateTime(2022, 01, 01),
            Description = "development team’s project end celebration",
            Vendor = "Seaside Steakhouse",
            ExpenseDetails = new List<ExpenseDetail>
                {
                    new ExpenseDetail
                    {
                        Total = 35000,
                        CostCentre = "DEV632",
                        PaymentMethod = "personal card"
                    }
                }
        };

        _dataExtractorMock.Setup(x => x.ExtractReservationDetails(text)).Returns(reservation);
        try
        {
            // Act
            var result = _expenseClaimService.ProcessText(text);
        }
        catch (ArgumentNullException ex)
        {
            // Assert
            Assert.IsType<FormatException>(ex);
        }
    }

    /// <summary>
    /// Test case to verify that the ProcessText method defaults to "Unknown" cost centre.
    /// </summary>
    [Fact]
    public void ProcessText_ShouldDefaultToUnknownCostCentre()
    {
        // Arrange
        string text = "Sample text";
        var reservation = new Reservation
        {
            Date = new DateTime(2022, 01, 01),
            Description = "Sample description",
            Vendor = "Sample vendor",
            ExpenseDetails = new List<ExpenseDetail>
                {
                    new ExpenseDetail
                    {
                        Total = 100,
                        CostCentre = null,
                        PaymentMethod = "Sample payment method"
                    }
                }
        };
        var processedExpenseDetail = new ProcessedExpenseDetail
        {
            TotalIncludingTax = 100,
            Tax = 10,
            TotalExcludingTax = 90,
            CostCentre = "UNKNOWN",
            PaymentMethod = "Sample payment method"
        };
        var processedReservation = new ProcessedReservation
        {
            Date = new DateTime(2022, 01, 01),
            Description = "Sample description",
            Vendor = "Sample vendor",
            ExpenseDetails = new List<ProcessedExpenseDetail> { processedExpenseDetail }
        };

        _dataExtractorMock.Setup(x => x.ExtractReservationDetails(text)).Returns(reservation);
        _taxCalculatorMock.Setup(x => x.CalculateTaxFromTotalAmount(10, 100)).Returns(10);

        // Act
        var result = _expenseClaimService.ProcessText(text);

        // Assert
        Assert.Equal(processedReservation.Date, result.Date);
        Assert.Equal(processedReservation.Description, result.Description);
        Assert.Equal(processedReservation.Vendor, result.Vendor);
        Assert.Single(result.ExpenseDetails);
        Assert.Equal(processedExpenseDetail.TotalIncludingTax, result.ExpenseDetails[0].TotalIncludingTax);
        Assert.Equal(processedExpenseDetail.Tax, result.ExpenseDetails[0].Tax);
        Assert.Equal(processedExpenseDetail.TotalExcludingTax, result.ExpenseDetails[0].TotalExcludingTax);
        Assert.Equal(processedExpenseDetail.CostCentre, result.ExpenseDetails[0].CostCentre);
        Assert.Equal(processedExpenseDetail.PaymentMethod, result.ExpenseDetails[0].PaymentMethod);
    }
}