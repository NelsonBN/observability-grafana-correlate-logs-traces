using Demo.Api.Domain;
using Demo.Api.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Api.UseCases;

public sealed record DeleteUserCommand(
    ILogger<CreateUserCommand> Logger,
    IUsersRepository Repository)
{
    private readonly ILogger<CreateUserCommand> _logger = Logger;
    private readonly IUsersRepository _repository = Repository;

    public async Task HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        using var activity = Telemetry.Source.StartActivity($"Command: {nameof(DeleteUserCommand)}");
        activity?.AddTag(nameof(id), id);

        if (!await _repository.AnyAsync(id, cancellationToken))
        {
            throw new UserNotFoundException(id);
        }

        await _repository.DeleteAsync(id, cancellationToken);

        _logger.LogInformation("[COMMAND][DELETE USER] user deleted");
    }
}
