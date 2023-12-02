using System.Reflection;
using Consumer.Application.Common.Behaviors;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Common.Concurrency;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Consumer.Application;

public static class DependencyInjectionRegister
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg
            => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddScoped<IValidatorChecks, ValidatorChecks>();
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }
}