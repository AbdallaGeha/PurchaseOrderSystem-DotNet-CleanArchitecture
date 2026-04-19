using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Purchase_Orders.Application.IQueries.Financial;
using Purchase_Orders.Application.IQueries.PurchaseOrders;
using Purchase_Orders.Application.IQueries.PurchaseOrderStatements;
using Purchase_Orders.Application.IQueries.Setup;
using Purchase_Orders.Application.IRepositories.Inventory;
using Purchase_Orders.Application.IRepositories.Payments;
using Purchase_Orders.Application.IRepositories.PurchaseOrders;
using Purchase_Orders.Application.IRepositories.PurchaseOrderStatements;
using Purchase_Orders.Application.IUOW;
using Purchase_Orders.Application.Services.Payments;
using Purchase_Orders.Application.Services.PurchaseOrders;
using Purchase_Orders.Application.Services.PurchaseOrderStatements;
using Purchase_Orders.Data;
using Purchase_Orders.Data.Queries.Financial;
using Purchase_Orders.Data.Queries.PurchaseOrders;
using Purchase_Orders.Data.Queries.PurchaseOrderStatements;
using Purchase_Orders.Data.Queries.Setup;
using Purchase_Orders.Data.Repositories.Inventory;
using Purchase_Orders.Data.Repositories.Payments;
using Purchase_Orders.Data.Repositories.PurchaseOrders;
using Purchase_Orders.Data.Repositories.PurchaseOrderStatements;
using Purchase_Orders.Data.UOW;
using PurchaseOrders.API.Middleware;
using PurchaseOrders.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddCors(x => x.AddDefaultPolicy(x => x.WithOrigins("http://localhost:4200")
    .AllowAnyMethod()
    .AllowAnyHeader()));

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddScoped<ILookupQuery, LookupQuery>();
builder.Services.AddScoped<IFinancialQuery, FinancialQuery>();
builder.Services.AddScoped<IPurchaseOrderQuery, PurchaseOrderQuery>();
builder.Services.AddScoped<IPurchaseOrderStatementQuery, PurchaseOrderStatementQuery>();

builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
builder.Services.AddScoped<IPurchaseOrderStatementRepository, PurchaseOrderStatementRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
builder.Services.AddScoped<IPurchaseOrderStatementService, PurchaseOrderStatementService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();


builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    //Add validation errors as one concatinated string to 400 response
    options.InvalidModelStateResponseFactory = context =>
    {
        var validationErrors = new List<string>();

        foreach (var state in context.ModelState)
        {
            foreach (var error in state.Value.Errors)
            {
                validationErrors.Add($"{state.Key}: {error.ErrorMessage}");
            }
        }

        return new BadRequestObjectResult(string.Join(System.Environment.NewLine, validationErrors));
    };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Run database seeding
await SeedDataRunner.RunAsync(app.Services);

app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
