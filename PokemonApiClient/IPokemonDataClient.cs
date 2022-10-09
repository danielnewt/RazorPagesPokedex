using RazerPagesPokedex.PokemonApiClient.Models;
using RazorPagesPokedex.PokemonApiClient.Models;

namespace RazorPagesPokedex.PokemonApiClient
{
    public interface IPokemonDataClient
    {
        Task<Pokemon> GetPokemonById(int id, CancellationToken cancellationToken = default);
        Task<Pokemon> GetPokemonByName(string name, CancellationToken cancellationToken = default);
        Task<PokemonList> GetPokemonList(int limit = 0, int offset = 0, CancellationToken cancellationToken = default);
    }
}
