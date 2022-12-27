using PokeApiClient.Client.Models;
using PokeApiClient.Client.Models.Pokemon;

namespace PokeApiClient.Client
{
    public interface IPokeApiClient
	{
		Task<Pokemon> GetPokemon(string id, CancellationToken cancellationToken = default);
		Task<ResourceList<Pokemon>> GetPokemonList(int limit = 0, int offset = 0, CancellationToken cancellationToken = default);
	}
}
