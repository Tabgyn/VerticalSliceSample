using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using VerticalSliceSample.Api.Database;

namespace VerticalSliceSample.IntegrationTests
{
    public class PostgreSqlContainerFixture : IAsyncLifetime
    {
        public PostgreSqlContainer Postgres { get; private set; }

        public PostgreSqlContainerFixture()
        {
            Postgres = new PostgreSqlBuilder()
                .WithImage("postgres:latest")
                .Build();
        }

        public async Task InitializeAsync()
        {
            await Postgres.StartAsync();

            // Ensure that the database schema is created by applying migrations.
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(Postgres.GetConnectionString())
                .Options;

            using var context = new ApplicationDbContext(options);
            await context.Database.MigrateAsync();
        }

        public Task DisposeAsync() => Postgres.DisposeAsync().AsTask();
    }
}
