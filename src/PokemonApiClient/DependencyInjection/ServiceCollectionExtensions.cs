using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PokeApiClient.Client;
using PokeApiClient.Options;

namespace PokeApiClient.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static void AddPokeApiClient(this IServiceCollection services, Action<PokeApiOptions> options)
	{
		services.AddOptions<PokeApiOptions>()
			.Configure(options)
			.ValidateOnStart();
		services.AddSingleton<IValidateOptions<PokeApiOptions>, ValidatePokeApiOptions>();
		services.AddSingleton<IPokeApiClient, Client.PokeApiClient>();
	}
}
