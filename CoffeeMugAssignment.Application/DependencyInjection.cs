using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using CoffeeMugAssignment.Application.Abstractions;
using CoffeeMugAssignment.Application.Common.Behaviors;
using CoffeeMugAssignment.Application.Orders.Services;

namespace CoffeeMugAssignment.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped<IOrderPricingService, OrderPricingService>();
        return services;
    }
}
