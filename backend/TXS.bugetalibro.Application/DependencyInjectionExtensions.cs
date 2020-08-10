using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TXS.bugetalibro.Application.Behaviours;

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
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        }
    }
}
