using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Infrastructure
{
    public sealed class AppServices
    {
        private static AppServices instance = null;
        private static IServiceProvider serviceProvider;
        private static readonly object _lock = new object();

        private AppServices()
        {
            var services = new ServiceCollection();

            services.AddTransient<ICustomerRepository, CustomersRepository>();
            services.AddTransient<IDbConnectionProvider, DbConnectionProvider>();
            services.AddTransient<ISqlExecutor, DapperSqlExecutor>();
            services.AddTransient<ILogger, FileLogger>(serviceProvider => new FileLogger("logs/log.txt"));
            services.AddTransient<IMessageService, MessageService>();

            serviceProvider = services.BuildServiceProvider();
        }

        public static AppServices Instance
        {
            get
            {
                lock (_lock)
                {
                    if (instance is null)
                    {
                        instance = new AppServices();
                    }

                    return instance;
                }
            }
        }

        public T Get<T>() where T : class
        {
            return serviceProvider.GetService(typeof(T)) as T;
        }
    }
}
