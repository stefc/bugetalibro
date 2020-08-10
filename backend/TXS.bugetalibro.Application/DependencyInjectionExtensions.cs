using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace TXS.bugetalibro.Application
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjectionExtensions).Assembly;

            return services
                .AddValidatorsFromAssembly(assembly)
                .AddMediatR(assembly);
            // .AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>))
            // .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>))
            // .AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
        }
    }
}
