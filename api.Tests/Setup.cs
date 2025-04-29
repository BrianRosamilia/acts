using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StargateAPI.Business.Data;

namespace StargateAPI.Tests
{
    public class Setup
    {
        private static WebApplicationFactory<Program> _server;

        public static WebApplicationFactory<Program> Get()
        {
            if (_server != null)
                return _server;

            return _server =
                new WebApplicationFactory<Program>()
                    .WithWebHostBuilder(builder =>
                        builder
                            .UseEnvironment("test")
                            .UseContentRoot(Directory.GetCurrentDirectory())
                                              .ConfigureServices(services =>
                            {
                                var descriptor = services.SingleOrDefault(
                                    d => d.ServiceType == typeof(DbContextOptions<StargateContext>));

                                if (descriptor != null)
                                {
                                    services.Remove(descriptor);
                                }

                                var connection = new Microsoft.Data.Sqlite.SqliteConnection("DataSource=:memory:");
                                connection.Open(); // Keep the connection open

                                services.AddDbContext<StargateContext>(options =>
                                {
                                    options.UseSqlite(connection);
                                });

                                var sp = services.BuildServiceProvider();
                                using var scope = sp.CreateScope();
                                var scopedServices = scope.ServiceProvider;
                                var db = scopedServices.GetRequiredService<StargateContext>();
                            })
                            );
        }
    }
}