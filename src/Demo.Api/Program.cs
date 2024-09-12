using Demo.Api.Domain;
using Demo.Api.Infrastructure;
using Demo.Api.UseCases;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reflection;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services
    .AddTransient<GetUsersQuery>()
    .AddTransient<GetUserQuery>()
    .AddTransient<CreateUserCommand>()
    .AddTransient<UpdateUserCommand>()
    .AddTransient<DeleteUserCommand>();

builder.Services
    .AddScoped<IUsersRepository, UsersRepository>();

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

builder.Services
    .Configure<RouteOptions>(options
        => options.ConstraintMap["regex"] = typeof(RegexInlineRouteConstraint));

builder.Services
    .AddOpenTelemetry()
    .UseOtlpExporter()
    .ConfigureResource(builder => builder
        .AddService(
            serviceName: Assembly.GetEntryAssembly()!.GetName().Name!,
            serviceVersion: Assembly.GetEntryAssembly()!.GetName().Version!.ToString())
        .AddTelemetrySdk())
    .WithTracing(builder => builder
        .AddSource(Telemetry.Source.Name)
        .AddAspNetCoreInstrumentation(o => o.RecordException = true))
    .WithLogging(builder => builder
    .SetResourceBuilder(ResourceBuilder.CreateDefault()
        .AddService(
            serviceName: Assembly.GetEntryAssembly()!.GetName().Name!,
            serviceVersion: Assembly.GetEntryAssembly()!.GetName().Version!.ToString())
        .AddTelemetrySdk()),
        configureOptions =>
        {
            configureOptions.IncludeFormattedMessage = true;
            configureOptions.IncludeScopes = true;
            configureOptions.ParseStateValues = true;
        });


var app = builder.Build();

app.UseSwagger()
   .UseSwaggerUI();

app.UseRouting();
app.MapUsersEndpoints();


await app.RunAsync();
