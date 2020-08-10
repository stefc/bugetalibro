using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TXS.Shared.Extensions;
using TXS.bugetalibro.Application.Behaviours;

namespace TXS.bugetalibro.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidators();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddMediatR(typeof(ServiceCollectionExtensions).Assembly);

            return services;
        }

        private static IServiceCollection AddValidators(this IServiceCollection services)
        {
            CollectServiceDescriptorsForInterface(typeof(IValidator<>))
                .ForEach(services.Add);
            return services;
        }

        private static IEnumerable<ServiceDescriptor> CollectServiceDescriptorsForInterface(Type openGenericType)
        {
            return from type in typeof(ServiceCollectionExtensions).Assembly.GetTypes()
                   where !type.IsAbstract && !type.IsGenericTypeDefinition
                   let interfaces = type.GetInterfaces()
                   let genericInterfaces = interfaces.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGenericType)
                   let matchingInterface = genericInterfaces.FirstOrDefault()
                   where matchingInterface != null
                   select ServiceDescriptor.Transient(matchingInterface, type);
        }
    }
}
