using RazorPagesPokedex.Models;

namespace RazorPagesPokedex.Services
{
    public interface IPokemonVmService
    {
        int PageCount { get; }
		Task<IEnumerable<PokemonVm>> GetPokemonListPage(int page, CancellationToken cancellationToken);
        Task<PokemonVm> GetPokemon(string name, CancellationToken cancellationToken);
    }
}
