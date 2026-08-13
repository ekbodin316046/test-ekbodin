using System.Reflection;
using Application.Common.Behaviours;
using Application.Features.IT.IT03.Common;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(assembly);

            // Order matters: validate before opening a transaction.
            configuration.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            configuration.AddOpenBehavior(typeof(TransactionBehaviour<,>));
        });

        services.AddValidatorsFromAssembly(assembly);
        services.AddScoped<DocumentDecisionExecutor>();

        return services;
    }
}
