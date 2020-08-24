using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TXS.bugetalibro.Application.Behaviours;
using TXS.bugetalibro.Application.Contracts.Data;
using TXS.bugetalibro.Domain.Entities;
using TXS.bugetalibro.Domain.Facades;

namespace TXS.bugetalibro.Application
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjectionExtensions).Assembly;

            return services
                .AddValidatorsFromAssembly(assembly)
                .AddMediatR(assembly)
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>))
                .AddTransient<BalanceQueryFacade>( sp => {
                        var dataStore = sp.GetRequiredService<IDataStore>();
                        var credits = dataStore.Set<Einzahlung>();
                        var debits = dataStore.Set<Auszahlung>();
                        return new BalanceQueryFacade(credits, debits);
                    });
        }
    }
}
