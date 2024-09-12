using Demo.Api.Domain;
using Demo.Api.DTOs;
using Demo.Api.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Api.UseCases;

public sealed record UpdateUserCommand(
    ILogger<CreateUserCommand> Logger,
    IUsersRepository Repository)
{
    private readonly ILogger<CreateUserCommand> _logger = Logger;
    private readonly IUsersRepository _repository = Repository;


    public async Task HandleAsync(Guid id, UserRequest request, CancellationToken cancellationToken)
    {
        using var activity = Telemetry.Source.StartActivity($"Command: {nameof(CreateUserCommand)}");
        activity?
            .AddTag(nameof(id), id)
            .AddTag(nameof(request.Name), request.Name)
            .AddTag(nameof(request.Email), request.Email)
            .AddTag(nameof(request.Phone), request.Phone);

        var user = await _repository
            .GetAsync(id, cancellationToken) ??
            throw new UserNotFoundException(id);

        user.Update(
            request.Name,
            request.Email,
            request.Phone);

        _logger.LogInformation("[COMMAND][UPDATE USER] user created");

        await _repository.UpdateAsync(user, cancellationToken);
    }
}
