using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Extensions;
using API.Services;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers();
        services.AddScoped<ToDoService>();
        services.AddScoped<UserService>();
        services.AddScoped<UserAuthService>();
        services.AddScoped<TokenService>();
        services.AddDbContext<AppDbContext>(options =>
               options.UseSqlServer(config.GetConnectionString("PracticeDb")));
        string base64Key = config["Encryption:Key"]
             ?? throw new InvalidOperationException("Encryption key not found in configuration.");
        CryptoExtensions.SetKey(base64Key);
        services.AddIdentityServices(config);
        return services;
    }
}
