using Demo.Api.Domain;
using Demo.Api.DTOs;
using Demo.Api.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Api.UseCases;

public sealed record GetUserQuery(
    ILogger<CreateUserCommand> Logger,
    IUsersRepository Repository)
{
    private readonly ILogger<CreateUserCommand> _logger = Logger;
    private readonly IUsersRepository _repository = Repository;

    public async Task<UserResponse> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        using var activity = Telemetry.Source.StartActivity($"Command: {nameof(GetUserQuery)}");
        activity?.AddTag(nameof(id), id);

        var users = await _repository
           .GetAsync(id, cancellationToken) ??
           throw new UserNotFoundException(id);

        _logger.LogInformation("[QUERY][GET USER] returned");

        return users;
    }
}
