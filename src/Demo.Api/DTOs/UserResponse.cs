using Demo.Api.Domain;
using System;

namespace Demo.Api.DTOs;

public sealed record UserResponse(
    Guid Id,
    string Name,
    string? Email,
    string? Phone)
{
    public static implicit operator UserResponse(User user)
    {
        return new(
            user.Id,
            user.Name,
            user.Email,
            user.Phone);
    }
}
