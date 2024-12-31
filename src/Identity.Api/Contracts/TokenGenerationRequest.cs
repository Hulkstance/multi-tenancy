namespace Identity.Api.Contracts;

public class TokenGenerationRequest
{
    public required Guid UserId { get; init; }

    public required string Email { get; init; }

    public Dictionary<string, object> CustomClaims { get; init; } = new();
}
