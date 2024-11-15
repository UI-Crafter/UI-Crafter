namespace UICrafter.Core.Extensions;

using System.Security.Claims;

public static class ClaimsPrincipalExtensions
{
	/// <summary>
	/// Retrieves the Azure AD object ID as a <see cref="Guid"/> from the <see cref="ClaimsPrincipal"/>.
	/// Checks for the "oid" or "http://schemas.microsoft.com/identity/claims/objectidentifier" claim types.
	/// </summary>
	/// <param name="principal">The <see cref="ClaimsPrincipal"/> to retrieve the Azure ID from.</param>
	/// <returns>A <see cref="Guid"/> representing the Azure AD object ID.</returns>
	/// <exception cref="FormatException">Thrown if the claim value cannot be parsed as a <see cref="Guid"/>.</exception>
	public static Guid GetAzureId(this ClaimsPrincipal principal)
	{
		return Guid.Parse(principal.GetClaimValue("oid", "http://schemas.microsoft.com/identity/claims/objectidentifier"));
	}

	/// <summary>
	/// Retrieves the username or name from the <see cref="ClaimsPrincipal"/>.
	/// Checks for "preferred_username", <see cref="ClaimsIdentity.DefaultNameClaimType"/>, and "name" claim types.
	/// </summary>
	/// <param name="principal">The <see cref="ClaimsPrincipal"/> to retrieve the name from.</param>
	/// <returns>A <see cref="string"/> representing the username or name.</returns>
	public static string GetName(this ClaimsPrincipal principal)
	{
		return principal.GetClaimValue("preferred_username", ClaimsIdentity.DefaultNameClaimType, "name");
	}

	/// <summary>
	/// Retrieves the email address from the <see cref="ClaimsPrincipal"/>.
	/// Checks for the "emails" or <see cref="ClaimTypes.Email"/> claim types.
	/// </summary>
	/// <param name="principal">The <see cref="ClaimsPrincipal"/> to retrieve the email address from.</param>
	/// <returns>A <see cref="string"/> representing the email address, or null if not present.</returns>
	public static string GetEmail(this ClaimsPrincipal principal)
	{
		return principal.GetClaimValue("emails", ClaimTypes.Email)!;
	}

	/// <summary>
	/// Retrieves the "AccessToken" value from the <see cref="ClaimsPrincipal"/> if it exists.
	/// </summary>
	/// <param name="principal">The <see cref="ClaimsPrincipal"/> to retrieve the access token from.</param>
	/// <returns>The access token as a <see cref="string"/>, or null if the "AccessToken" claim is not present.</returns>
	public static string? GetAccessToken(this ClaimsPrincipal principal)
	{
		return principal.GetClaimValue("AccessToken");
	}

	/// <summary>
	/// Retrieves the value of the first matching claim from the <see cref="ClaimsPrincipal"/>.
	/// Iterates through the specified claim names and returns the value of the first non-null match.
	/// </summary>
	/// <param name="claimsPrincipal">The <see cref="ClaimsPrincipal"/> to retrieve the claim from.</param>
	/// <param name="claimNames">An array of claim type names to search for.</param>
	/// <returns>A <see cref="string"/> containing the value of the first matched claim.</returns>
	/// <exception cref="ClaimNotFoundException">Thrown if no matching claim is found.</exception>
	private static string GetClaimValue(this ClaimsPrincipal claimsPrincipal, params string[] claimNames)
	{
		for (var i = 0; i < claimNames.Length; i++)
		{
			var currentValue = claimsPrincipal.FindFirstValue(claimNames[i]);
			if (!string.IsNullOrEmpty(currentValue))
			{
				return currentValue;
			}
		}

		throw new ClaimNotFoundException("No claim could be found matching parameters");
	}

	/// <summary>
	/// Returns the value for the first claim of the specified type, otherwise null if the claim is not present.
	/// </summary>
	/// <param name="principal">The <see cref="ClaimsPrincipal"/> instance this method extends.</param>
	/// <param name="claimType">The claim type whose first value should be returned.</param>
	/// <returns>The value of the first instance of the specified claim type, or null if the claim is not present.</returns>
	public static string? FindFirstValue(this ClaimsPrincipal principal, string claimType)
	{
		var claim = principal.FindFirst(claimType);
		return claim?.Value;
	}

}

/// <summary>
/// Exception thrown when a specified claim is not found in the <see cref="ClaimsPrincipal"/>.
/// </summary>
public class ClaimNotFoundException : Exception
{
	public ClaimNotFoundException()
	{
	}

	public ClaimNotFoundException(string message)
		: base(message)
	{
	}

	public ClaimNotFoundException(string message, Exception inner)
		: base(message, inner)
	{
	}
}
