﻿using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TXS.bugetalibro.Application.Behaviours;
using TXS.bugetalibro.Application.Contracts;
using TXS.bugetalibro.Infrastructure.Services;

namespace TXS.bugetalibro.Infrastructure
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            return services
                .AddTransient<IDateProvider, SystemDateProvider>();
        }
    }
}
