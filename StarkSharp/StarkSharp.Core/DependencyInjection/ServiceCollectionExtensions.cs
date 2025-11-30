using Microsoft.Extensions.DependencyInjection;
using StarkSharp.Core.Configuration;
using StarkSharp.Core.Connector;
using StarkSharp.Core.Interfaces;
using StarkSharp.Core.Services;

namespace StarkSharp.Core.DependencyInjection
{
    /// <summary>
    /// Extension methods for service collection registration
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds StarkSharp services to the service collection
        /// </summary>
        public static IServiceCollection AddStarkSharp(this IServiceCollection services, StarkSharpOptions options = null)
        {
            // Register configuration
            if (options != null)
            {
                services.AddSingleton<IStarkSharpConfiguration>(options);
            }
            else
            {
                services.AddSingleton<IStarkSharpConfiguration, StarkSharpOptions>();
            }

            // Register services
            services.AddScoped<ILoggingService, LoggingService>();
            services.AddScoped<IRpcService, RpcService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IContractService, ContractService>();
            services.AddScoped<IBlockchainService, BlockchainService>();

            // Register connector
            services.AddScoped<IConnector, StarkSharpConnector>();

            return services;
        }

        /// <summary>
        /// Adds StarkSharp services with platform implementation
        /// </summary>
        public static IServiceCollection AddStarkSharp<TPlatform>(this IServiceCollection services, StarkSharpOptions options = null)
            where TPlatform : class, IPlatform
        {
            services.AddStarkSharp(options);
            services.AddSingleton<IPlatform, TPlatform>();
            return services;
        }

        /// <summary>
        /// Adds StarkSharp services with platform instance
        /// </summary>
        public static IServiceCollection AddStarkSharp(this IServiceCollection services, IPlatform platform, StarkSharpOptions options = null)
        {
            services.AddStarkSharp(options);
            services.AddSingleton(platform);
            return services;
        }
    }
}
