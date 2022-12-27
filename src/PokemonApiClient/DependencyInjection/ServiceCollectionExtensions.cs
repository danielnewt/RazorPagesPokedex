using Microsoft.Extensions.DependencyInjection;
using PokeApiClient.Client;

namespace PokeApiClient.DependencyInjection
{
	public static class ServiceCollectionExtensions
	{
		public static void AddPokeApiClient(this IServiceCollection services, Action<PokeApiOptions> options)
		{
			services.Configure(options);
			services.AddSingleton<IPokeApiClient, Client.PokeApiClient>();
		}
	}
}
