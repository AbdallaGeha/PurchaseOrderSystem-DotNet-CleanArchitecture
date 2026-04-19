using Purchase_Orders.Data;

namespace PurchaseOrders.Data
{
    public class SeedDataRunner
    {
        public static async Task RunAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var dbContext = services.GetRequiredService<ApplicationDbContext>();
                await DbSeeder.SeedAsync(dbContext);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<SeedDataRunner>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }
    }
}
