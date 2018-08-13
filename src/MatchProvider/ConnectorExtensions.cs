using System;
using System.Collections.Generic;
using System.Text;
using MatchProvider.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace MatchProvider
{
    public static class ConnectorExtensions
    {
        /// <summary>
        /// Adds the connectors.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        public static void AddConnectors(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IFussballDataRepository, FussballDataRepository>();
            serviceCollection.AddSingleton<ICacheProvider,DefaultCacheProvider>();
        }
    }
}
