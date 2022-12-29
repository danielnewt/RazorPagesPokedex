using PokeApiClient.Client;
using PokeApiClient.Client.Models;
using PokeApiClient.Client.Models.Pokemon;

namespace PokeApiClient.Extensions
{
	public static class PokeApiClientExtensions
	{
		/// <summary>
		/// Convenience method to get Pokemon by Id.
		/// </summary>
		/// <param name="client">IPokeApiClient implementation.</param>
		/// <param name="id">The Pokemon Id.</param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public static async Task<Pokemon> GetPokemon(
			this IPokeApiClient client, 
			string id, 
			CancellationToken cancellationToken = default) => 
			await client.GetResourceById<Pokemon>(id, cancellationToken);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="client">IPokeApiClient implementation.</param>
		/// <param name="limit">Limit request parameter.</param>
		/// <param name="offset">Offset request parameter.</param>
		/// <param name="includeResource">If true the Pokemon Resources will be requested as well.</param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public static async Task<ResourceList<Pokemon>> GetPokemonList(
			this IPokeApiClient client, 
			int limit = 20, 
			int offset = 0, 
			bool includeResource = false, 
			CancellationToken cancellationToken = default) => 
			await client.GetResourceList<Pokemon>(limit, offset, includeResource, cancellationToken);
	}
}
