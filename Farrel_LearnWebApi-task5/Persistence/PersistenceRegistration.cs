using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence.DatabaseContext;
using Persistence.Repositories;
using StackExchange.Redis;

namespace Persistence
{
    public static class PersistenceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
        {
            const string dbConnection = "Server=localhost;Database=master;Trusted_Connection=True;Encrypt=False;";
            const string redisConnection = "localhost:6379";

            services.AddDbContext<TableContext>(opt => opt.UseSqlServer(dbConnection));
            services.AddScoped<ITableSpecificationRepository, TableSpecificationRepository>();

            //TryCatch untuk konek tanpa Redis
            try
            {
                var redis = ConnectionMultiplexer.Connect(redisConnection);
                services.AddSingleton<IConnectionMultiplexer>(redis);
            }
            catch (RedisConnectionException ex)
            {
                Console.WriteLine($"Redis connection failed: {ex.Message}. Continuing without Redis.");

                services.AddSingleton<IConnectionMultiplexer>(sp => null);
            }

            return services;
        }
    }
}
