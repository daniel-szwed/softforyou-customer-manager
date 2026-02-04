using Domain.Repositories;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Infrastructure
{
    public static class AppServices
    {
        private static IServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();

            services.AddTransient<ICustomerRepository, CustomersRepository>();
            services.AddTransient<IDbConnectionProvider, DbConnectionProvider>();
            services.AddTransient<ISqlExecutor, DapperSqlExecutor>();

            return services.BuildServiceProvider();
        }

        public static T Get<T>() where T : class
        {
            return CreateServiceProvider().GetService(typeof(T)) as T;
        }
    }
}
