using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VerticalSliceSample.Api.Database;

namespace VerticalSliceSample.IntegrationTests
{
    public class CustomWebApplicationFactory(string postgreSqlConnectionString) : WebApplicationFactory<Program>
    {
        private readonly string postgreSqlConnectionString = postgreSqlConnectionString;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Overwrite the existing DB context registration so that tests use the PostgreSQL container.
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseNpgsql(postgreSqlConnectionString);
                });
            });
        }
    }
}
