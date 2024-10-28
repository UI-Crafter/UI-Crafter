namespace UICrafter.Extensions;

using System.Security.Claims;
using Microsoft.Identity.Web;

public static class ClaimsPrincipalExtensions
{
	public static Guid GetAzureId(this ClaimsPrincipal principal)
	{
		return Guid.Parse(principal.GetObjectId()!);
	}

	public static string GetName(this ClaimsPrincipal principal)
	{
		return principal.GetDisplayName()!;
	}

	public static string GetEmail(this ClaimsPrincipal principal)
	{
		return principal.GetClaimValue("emails", ClaimTypes.Email)!;
	}

	private static string GetClaimValue(this ClaimsPrincipal? claimsPrincipal, params string[] claimNames)
	{
		ArgumentNullException.ThrowIfNull(claimsPrincipal);

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
}
