using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StayWize.Infrastructure.Jobs;
using StayWize.Infrastructure.Persistence.Context;

namespace StayWize.IntegrationTests.Setup;

public class StayWizeWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = "StayWizeTestDb_" + Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.Sources.Clear();
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "InMemory",
                ["JwtSettings:SecretKey"] = "staywize-test-secret-key-must-be-32-chars!!",
                ["JwtSettings:Issuer"] = "StayWize",
                ["JwtSettings:Audience"] = "StayWizeUsers",
                ["JwtSettings:ExpirationHours"] = "24",
                ["EncryptionSettings:SecretKey"] = "staywize-test-encryption-key-32bytes",
                ["SmtpSettings:Host"] = "localhost",
                ["SmtpSettings:Port"] = "25",
                ["SmtpSettings:Username"] = "test",
                ["SmtpSettings:Password"] = "test",
                ["SmtpSettings:FromName"] = "StayWize Test",
                ["SmtpSettings:FromEmail"] = "test@staywize.com",
                ["JobSettings:AccessCodeExpirationIntervalMinutes"] = "60"
            });
        });

        builder.ConfigureServices(services =>
        {
            // Reemplazar SQL Server por InMemory con nombre único por instancia
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));

            // Remover el background job
            var jobDescriptor = services.SingleOrDefault(
                d => d.ImplementationType == typeof(AccessCodeExpirationJob));
            if (jobDescriptor != null)
                services.Remove(jobDescriptor);
        });

        builder.UseEnvironment("Testing");
    }
}