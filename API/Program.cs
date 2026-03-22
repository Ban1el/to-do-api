using API.Data.Seed;
using API.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Error()
    .WriteTo.File(
        path: "Logs/Errors/error-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "[{Timestamp:HH:mm:ss}] ERROR{NewLine}{Message:l}{NewLine}{Exception}------------------------------------------------------------{NewLine}{NewLine}")
    .CreateLogger();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    UserSeedData.Initialize(scope.ServiceProvider);
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<AuditTrailMiddleware>();

app.MapControllers();

app.Run();
