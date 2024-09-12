namespace Demo.Api.DTOs;

public sealed record UserRequest(
    string Name,
    string? Email,
    string? Phone);
