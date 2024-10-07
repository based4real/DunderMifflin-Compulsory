using API.ExceptionHandler;
using DataAccess;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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

builder.Services.AddScoped<IPaperRepository, PaperRepository>();

builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaperService, PaperService > ();

builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                });
                
                
builder.Services.AddOpenApiDocument(configure =>
{
    configure.Title = "Dunder Mifflin API";
    configure.Version = "v1";
    configure.Description = "API for Dunder Mifflin paper shop";
});

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = ctx => ctx.ProblemDetails.Extensions.Add("instance", $"{ctx.HttpContext.Request.Method} {ctx.HttpContext.Request.Path}");
});

builder.Services.AddExceptionHandler<ExceptionToProblemDetailsHandler>();

builder.Services.AddHealthChecks()
                .AddDbContextCheck<AppDbContext>("Database", failureStatus: HealthStatus.Unhealthy)
                .AddCheck("API", () => HealthCheckResult.Healthy("The application is healthy."));

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

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = JsonConvert.SerializeObject(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description,
            }),
        });
        await context.Response.WriteAsync(result);
    }
});

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
}

app.Run();

public partial class Program;