using AutoMapper;
using PokeApiClient.Client;
using RazorPagesPokedex.Models;

namespace RazorPagesPokedex.Services
{
	public class PokemonVmService : IPokemonVmService
    {
		public const int PokemonLimit = 151;
		public const int PokemonPerPage = 6;
		public int PageCount { get; } = (int)Math.Ceiling((double)PokemonLimit / PokemonPerPage);

		private readonly IPokeApiClient _dataClient;
		private readonly IMapper _mapper;

		public PokemonVmService(
			IPokeApiClient dataClient,
			IMapper mapper)
		{
			_dataClient = dataClient;
			_mapper = mapper;
		}

		public async Task<PokemonVm> GetPokemon(string name, CancellationToken cancellationToken)
        {
			var pokemon = await _dataClient.GetPokemon(name, cancellationToken);
			return _mapper.Map<PokemonVm>(pokemon);
        }

        public async Task<IEnumerable<PokemonVm>> GetPokemonListPage(int page, CancellationToken cancellationToken)
        {
			var offset = (page - 1) * PokemonPerPage;
			var limit = Math.Min(PokemonPerPage, PokemonLimit - offset);

			var pokemonList = await _dataClient.GetPokemonList(limit, offset, cancellationToken);

			return pokemonList.Results.Select(x => _mapper.Map<PokemonVm>(x.Resource)).ToArray();
		}
	}
}
