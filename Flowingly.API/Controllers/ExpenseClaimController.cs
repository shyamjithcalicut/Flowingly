using Flowingly.Contracts.ExpenseClaim;
using Flowingly.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Flowingly.API.Controllers;

/// <summary>
/// Represents the controller for managing expense claims.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class ExpenseClaimController : ControllerBase
{
    private readonly IExpenseClaimService _expenseClaimService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpenseClaimController"/> class.
    /// </summary>
    /// <param name="expenseClaimService">The expense claim service.</param>
    public ExpenseClaimController(IExpenseClaimService expenseClaimService)
    {
        _expenseClaimService = expenseClaimService;
    }

    /// <summary>
    /// Processes the provided text and returns the processed reservation.
    /// </summary>
    /// <param name="text">The text to process.</param>
    /// <returns>The processed reservation.</returns>
    [HttpGet]
    [SwaggerOperation("ProcessText")]
    [SwaggerResponse(200, "OK", typeof(ProcessedReservation))]
    [SwaggerResponse(400, "Bad Request")]
    public IActionResult ProcessText(string text)
    {
        var processedReservation = _expenseClaimService.ProcessText(text);
        return Ok(processedReservation);
    }
}