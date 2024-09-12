using System;

namespace Demo.Api.Domain;

public sealed class UserNotFoundException(Guid Id)
    : Exception($"User with id '{Id}' was not found")
{ }
