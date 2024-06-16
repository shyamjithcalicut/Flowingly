using Flowingly.API.Middlewares;
using Flowingly.Contracts.DataExtractor;
using Flowingly.Contracts.ExpenseClaim;
using Flowingly.Contracts.TaxCalculator;
using Flowingly.Services.DataExtractor;
using Flowingly.Services.ExpenseClaim;
using Flowingly.Services.TaxCalculator;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IReservationDataExtractor, XmlReservationDataExtractor>();
builder.Services.AddSingleton<ITaxCalculator, TaxCalculator>();
builder.Services.AddSingleton<IExpenseClaimService, ExpenseClaimService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseMiddleware<ExceptionMiddleware>();

app.Run();