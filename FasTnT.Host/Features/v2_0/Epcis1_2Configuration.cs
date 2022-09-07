using FasTnT.Application.Queries;
using FasTnT.Application.Services;
using FasTnT.Application.Services.Users;
using FasTnT.Domain.Infrastructure.Behaviors;
using FasTnT.Host.Services.Subscriptions;
using FasTnT.Host.Services.User;
using FasTnT.Infrastructure.Configuration;
using FasTnT.Subscriptions;
using FasTnT.Subscriptions.Notifications;
using FluentValidation;
using MediatR;

namespace FasTnT.Host.Features.v2_0;

public static class Epcis2_0Configuration
{
    public static IEndpointRouteBuilder MapEpcis20Endpoints(this IEndpointRouteBuilder endpoints)
    {
        Endpoints2_0.AddRoutes(endpoints);

        return endpoints;
    }
}
