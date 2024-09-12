using Demo.Api.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Api.Infrastructure;

public sealed class UsersRepository : IUsersRepository
{
    private static readonly Dictionary<Guid, User> _users = [];

    public Task<IEnumerable<User>> ListAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<User>>(_users.Values);
    }

    public Task<User?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (_users.TryGetValue(id, out var user))
        {
            return Task.FromResult<User?>(user);
        }

        return Task.FromResult<User?>(default);
    }

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        _users.Add(user.Id, user);
        return Task.CompletedTask;
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _users[user.Id] = user;
        await Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _users.Remove(id);
        return Task.CompletedTask;
    }

    public Task<bool> AnyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_users.Any(n => n.Key == id));
    }
}
