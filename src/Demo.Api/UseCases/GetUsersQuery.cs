using Demo.Api.Domain;
using Demo.Api.DTOs;
using Demo.Api.Infrastructure;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Api.UseCases;

public sealed record GetUsersQuery(
    ILogger<CreateUserCommand> Logger,
    IUsersRepository Repository)
{
    private readonly ILogger<CreateUserCommand> _logger = Logger;
    private readonly IUsersRepository _repository = Repository;

    public async Task<IEnumerable<UserResponse>> HandleAsync(CancellationToken cancellationToken)
    {
        using var activity = Telemetry.Source.StartActivity($"Command: {nameof(GetUsersQuery)}");

        var users = await _repository.ListAsync(cancellationToken);

        var result = users.Select(n => (UserResponse)n);

        _logger.LogInformation("[QUERY][GET USERS] total: {Total}", result.Count());

        return result;
    }
}
