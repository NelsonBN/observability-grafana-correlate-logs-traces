using Demo.Api.Domain;
using Demo.Api.DTOs;
using Demo.Api.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Api.UseCases;

public sealed record CreateUserCommand(
    ILogger<CreateUserCommand> Logger,
    IUsersRepository Repository)
{
    private readonly ILogger<CreateUserCommand> _logger = Logger;
    private readonly IUsersRepository _repository = Repository;

    public async Task<Guid> HandleAsync(UserRequest request, CancellationToken cancellationToken)
    {
        using var activity = Telemetry.Source.StartActivity($"Command: {nameof(CreateUserCommand)}");
        activity?
            .AddTag(nameof(request.Name), request.Name)
            .AddTag(nameof(request.Email), request.Email)
            .AddTag(nameof(request.Phone), request.Phone);

        var user = User.Create(
            request.Name,
            request.Email,
            request.Phone);

        await _repository.AddAsync(user, cancellationToken);

        _logger.LogInformation("[COMMAND][CREATE USER] user created");

        return user.Id;
    }
}
