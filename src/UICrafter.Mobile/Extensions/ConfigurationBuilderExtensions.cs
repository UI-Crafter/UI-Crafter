namespace UICrafter.Mobile.Extensions;

using System.Reflection;
using Microsoft.Extensions.Configuration;

public static class ConfigurationBuilderExtensions
{
	/// <summary>
	/// Adds an embedded json resource so the configuration builder.
	/// The name parameter should be th name of the resource in the format projectname.resource
	/// </summary>
	/// <param name="builder"></param>
	/// <param name="name"></param>
	/// <returns></returns>
	public static IConfigurationBuilder AddEmbeddedResource(this IConfigurationBuilder builder, string name)
	{
		var asm = Assembly.GetExecutingAssembly();
		using var stream = asm.GetManifestResourceStream(name);

		if (stream == null)
		{
			throw new FileNotFoundException($"Embedded resource '{name}' not found in assembly '{asm.FullName}'.");
		}

		using var reader = new StreamReader(stream);
		var jsonContent = reader.ReadToEnd();

		// Convert the string to a new MemoryStream so it remains accessible during the .Build() call
		var jsonStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonContent));

		return builder.AddJsonStream(jsonStream);
	}

}
