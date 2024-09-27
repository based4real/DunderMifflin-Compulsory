using API.ExceptionHandler;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Service;
using Service.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Services.AddOptions<AppOptions>()
                .Bind(builder.Configuration.GetSection(nameof(AppOptions)))
                .ValidateDataAnnotations()
                .ValidateOnStart();

builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var appOptions = serviceProvider.GetRequiredService<IOptions<AppOptions>>().Value;
    options.UseNpgsql(Environment.GetEnvironmentVariable("LocalDbConn") ?? appOptions.LocalDbConn);
});

builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddControllers();
builder.Services.AddOpenApiDocument(configure =>
{
    configure.Title = "Dunder Mifflin API";
    configure.Version = "v1";
    configure.Description = "API for Dunder Mifflin paper shop";
});

builder.Services.AddProblemDetails(options =>
options.CustomizeProblemDetails = ctx =>
{
    ctx.ProblemDetails.Extensions.Add("instance", $"{ctx.HttpContext.Request.Method} {ctx.HttpContext.Request.Path}");
});

builder.Services.AddExceptionHandler<ExceptionToProblemDetailsHandler>();

var app = builder.Build();

// Middleware
app.UseStaticFiles();

app.UseOpenApi();
app.UseSwaggerUi(settings =>
{ 
    settings.DocumentTitle = "Dunder Mifflin API";
    settings.DocExpansion = "list";
    settings.CustomStylesheetPath = "/swagger-ui/universal-dark.css";
});
app.UseStatusCodePages();
app.UseExceptionHandler();

app.UseCors(config => config.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
}

app.Run();

public partial class Program;