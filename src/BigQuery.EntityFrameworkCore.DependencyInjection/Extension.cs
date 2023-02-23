using Google.Cloud.BigQuery.V2;
using BigQuery.EntityFrameworkCore;
using BigQuery.EntityFrameworkCore.Utils;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class Extension
    {
        public static IServiceCollection AddBqContext<TBqContext>(this IServiceCollection services, Action<BqContextOptions> options) where TBqContext : BqContext
        {
            var datasets = typeof(TBqContext).GetProperties()
                .Where(prop => typeof(Dataset).IsAssignableFrom(prop.PropertyType))
                .ToArray();

            var tablesByDataset = datasets.ToDictionary(
                x => x.PropertyType,
                x => x.PropertyType.GetProperties().Where(prop => typeof(Table).IsAssignableFrom(prop.PropertyType)).ToArray());

            var bqContextOptions = new BqContextOptions();
            options.Invoke(bqContextOptions);

            services.AddSingleton(bqContextOptions)
                    .AddSingleton<IQueryProvider, BigQueryProvider>()
                    .AddSingleton<BigQueryRowParser>()
                    .AddTransient<IExpressionPrinter, BigQueryExpressionVisitor>()
                    .AddSingleton<BigQueryClientFactory>(x => BigQueryClient.Create)
                    .AddTransient<IExecuteQuery, ExecuteQuery>()
                    .AddScoped<BqContext, TBqContext>()
                    .AddTables(tablesByDataset)
                    .AddDatasetsAndInitializeTables(datasets, tablesByDataset)
                    .AddBqContextAndInitializeDatasets<TBqContext>(datasets);

            return services;
        }

        private static IServiceCollection AddBqContextAndInitializeDatasets<T>(this IServiceCollection services, PropertyInfo[] datasets) where T : BqContext
        {
            services.AddScoped(x =>
            {
                var bqContext = x.GetRequiredService<BqContext>();

                foreach (var dataset in datasets)
                    dataset.SetValue(bqContext, x.GetRequiredService(dataset.PropertyType));

                return (T)bqContext;
            });

            return services;
        }

        private static IServiceCollection AddDatasetsAndInitializeTables(this IServiceCollection services, PropertyInfo[] datasets, Dictionary<Type, PropertyInfo[]> tablesByDataset)
        {
            var datasetBaseType = typeof(Dataset<>);

            foreach (var dataset in datasets)
                services.AddScoped(datasetBaseType.MakeGenericType(dataset.PropertyType), dataset.PropertyType);

            foreach (var dataset in datasets)
                services.AddScoped(dataset.PropertyType, x =>
                {
                    var datasetInstance = x.GetRequiredService(datasetBaseType.MakeGenericType(dataset.PropertyType));

                    if (tablesByDataset.TryGetValue(dataset.PropertyType, out var tables))
                        foreach (var table in tables)
                            table.SetValue(datasetInstance, x.GetRequiredService(table.PropertyType));

                    return datasetInstance;
                });

            return services;
        }

        private static IServiceCollection AddTables(this IServiceCollection services, Dictionary<Type, PropertyInfo[]> tablesByDataset)
        {
            foreach (var tables in tablesByDataset)
                foreach (var table in tables.Value)
                {
                    if (services.Any(x => x.ServiceType == table.PropertyType))
                        throw new ArgumentException(string.Format(ErrorMessages.DuplicateTableErrorMessage, table.Name));

                    services.AddScoped(table.PropertyType, x =>
                    {
                        var datasetAttribute = tables.Key.GetCustomAttribute<DatasetAttribute>();
                        var datasetName = datasetAttribute?.DatasetName ?? tables.Key.Name;

                        var tableAttribute = table.PropertyType.GenericTypeArguments[0].GetCustomAttribute<TableAttribute>();
                        var tableName = tableAttribute?.Name ?? table.Name;

                        return Activator.CreateInstance(table.PropertyType, 
                            x.GetRequiredService<IQueryProvider>(),
                            x.GetRequiredService<IExpressionPrinter>(), 
                            datasetName, 
                            tableName) ?? throw new ArgumentException(string.Format(ErrorMessages.WasNotPossibleCreateErrorMessage, table.PropertyType));
                    });
                }

            return services;
        }
    }
}
