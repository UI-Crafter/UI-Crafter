namespace UICrafter.Client;

using System.Security.Claims;

// Add properties to this class and update the server and client AuthenticationStateProviders
// to expose more information about the authenticated user to the client.
public sealed class UserInfo
{
	public required IEnumerable<SimpleClaim> Claims { get; init; }

	public static UserInfo FromClaimsPrincipal(ClaimsPrincipal principal)
	{
		var user = new UserInfo() { Claims = GetRequiredClaim(principal) };
		return user;
	}

	public ClaimsPrincipal ToClaimsPrincipal() => new(new ClaimsIdentity(Claims.Select(x => new Claim(x.Type, x.Value)), nameof(UserInfo)));

	private static IEnumerable<SimpleClaim> GetRequiredClaim(ClaimsPrincipal principal) => principal.Claims.Select(x => new SimpleClaim() { Type = x.Type, Value = x.Value }).ToList() ?? throw new InvalidOperationException("Could not find any claims.");
}

public struct SimpleClaim
{
	public required string Type { get; set; }
	public required string Value { get; set; }
}
