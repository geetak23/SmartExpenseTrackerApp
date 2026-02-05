using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartExpenseTracker.Core.Interfaces;
using SmartExpenseTracker.Infrastructure.Data;
using SmartExpenseTracker.Infrastructure.Repositories;
using SmartExpenseTracker.Infrastructure.Services;

namespace SmartExpenseTracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(opt =>opt.UseNpgsql(config.GetConnectionString("DefaultConnection")));

        services.AddScoped<BlobStorageService>();
        services.AddScoped<ReceiptAnalysisService>();
        services.AddScoped<ReceiptRepository>();

        return services;
    }
}