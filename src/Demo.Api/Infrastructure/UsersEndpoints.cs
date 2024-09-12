using Demo.Api.Domain;
using Demo.Api.DTOs;
using Demo.Api.UseCases;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Demo.Api.Infrastructure;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/users")
            .WithOpenApi();


        group.MapGet("", async (ILoggerFactory logger, GetUsersQuery query, CancellationToken cancellationToken) =>
        {
            try
            {
                using var activity = Activity.Current?.SetTag("List", "All");

                var response = await query.HandleAsync(cancellationToken);

                logger
                    .CreateLogger("GET: /users")
                    .LogInformation("[GET][USERS] total: {Total}", response.Count());

                return Results.Ok(response);
            }
            catch (Exception exception)
            {
                logger
                    .CreateLogger("GET: /users")
                    .LogError(exception, "[GET][USERS] Error");

                return Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    detail: exception.Message);
            }
        });


        group.MapGet("{id:guid}", async (ILoggerFactory logger, GetUserQuery query, Guid id, CancellationToken cancellationToken) =>
        {
            try
            {
                using var activity = Activity.Current?.SetTag("UserId", id.ToString());

                var response = await query.HandleAsync(id, cancellationToken);

                logger
                    .CreateLogger("GET: /users/{id}")
                    .LogInformation("[GET][USER] {Id}", id);


                return Results.Ok(response);
            }
            catch (UserNotFoundException exception)
            {
                logger
                    .CreateLogger("GET: /users/{id}")
                    .LogWarning("[GET][USER] Not found {Id}", id);

                return Results.NotFound(exception.Message);
            }
            catch (Exception exception)
            {
                logger
                    .CreateLogger("GET: /users/{id}")
                    .LogError(exception, "[GET][USER] Error {Id}", id);

                return Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    detail: exception.Message);
            }
        }).WithName("GetUser");


        group.MapPost("", async (ILoggerFactory logger, CreateUserCommand command, UserRequest request, CancellationToken cancellationToken) =>
        {
            try
            {
                var id = await command.HandleAsync(request, cancellationToken);

                using var activity = Activity.Current?.SetTag("UserId", id.ToString());

                logger
                    .CreateLogger("POST: /users")
                    .LogInformation("[POST][USER] {Id}", id);

                return Results.CreatedAtRoute(
                    "GetUser",
                    new { id },
                    id);
            }
            catch (Exception exception)
            {
                logger
                    .CreateLogger("POST: /users/{id}")
                    .LogError(exception, "[POST][USER] Error");

                return Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    detail: exception.Message);
            }
        });


        group.MapPut("{id:guid}", async (ILoggerFactory logger, UpdateUserCommand command, Guid id, UserRequest request, CancellationToken cancellationToken) =>
        {
            try
            {
                using var activity = Activity.Current?.SetTag("UserId", id.ToString());

                await command.HandleAsync(id, request, cancellationToken);

                logger
                    .CreateLogger("PUT: /users/{id}")
                    .LogInformation("[PUT][USER] {Id}", id);

                return Results.NoContent();
            }
            catch (UserNotFoundException exception)
            {
                logger
                    .CreateLogger("PUT: /users/{id}")
                    .LogWarning("[PUT][USER] Not found {Id}", id);

                return Results.NotFound(exception.Message);
            }
            catch (Exception exception)
            {
                logger
                    .CreateLogger("PUT: /users/{id}")
                    .LogError(exception, "[PUT][USER] Error {Id}", id);

                return Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    detail: exception.Message);
            }
        });


        group.MapDelete("{id:guid}", async (ILoggerFactory logger, DeleteUserCommand command, Guid id, CancellationToken cancellationToken) =>
        {
            try
            {
                using var activity = Activity.Current?.SetTag("UserId", id.ToString());

                await command.HandleAsync(id, cancellationToken);

                logger
                    .CreateLogger("DELETE: /users/{id}")
                    .LogInformation("[DELETE][USER] {Id}", id);

                return Results.NoContent();
            }
            catch (UserNotFoundException exception)
            {
                logger
                    .CreateLogger("DELETE: /users/{id}")
                    .LogWarning("[DELETE][USER] Not found {Id}", id);

                return Results.NotFound(exception.Message);
            }
            catch (Exception exception)
            {
                logger
                    .CreateLogger("DELETE: /users/{id}")
                    .LogError(exception, "[DELETE][USER] Error {Id}", id);

                return Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    detail: exception.Message);
            }
        });
    }
}
