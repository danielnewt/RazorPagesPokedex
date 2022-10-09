using AutoMapper;
using RazorPagesPokedex.Models;
using RazorPagesPokedex.PokemonApiClient;
using System.Runtime.InteropServices;
using System.Xml;

namespace RazorPagesPokedex.Services
{
	public class PokemonVmService : IPokemonVmService
    {
		public const int PokemonLimit = 151;
		public const int PokemonPerPage = 6;
		public int PageCount { get; } = (int)Math.Ceiling((double)PokemonLimit / PokemonPerPage);

		private readonly IPokemonDataClient _dataClient;
		private readonly IMapper _mapper;

		public PokemonVmService(
			IPokemonDataClient dataClient,
			IMapper mapper)
		{
			_dataClient = dataClient;
			_mapper = mapper;
		}

		public async Task<PokemonVm> GetPokemon(string name, CancellationToken cancellationToken)
        {
			var pokemon = await _dataClient.GetPokemonByName(name, cancellationToken);
			return _mapper.Map<PokemonVm>(pokemon);
        }

        public async Task<IEnumerable<PokemonVm>> GetPokemonListPage(int page, CancellationToken cancellationToken)
        {
			var offset = (page - 1) * PokemonPerPage;
			var limit = Math.Min(PokemonPerPage, PokemonLimit - offset);
			var pokemonList = await _dataClient.GetPokemonList(limit, offset, cancellationToken);

			var vmList = new List<PokemonVm>(PokemonPerPage);
			foreach (var i in pokemonList.Results)
			{
				vmList.Add(await GetPokemon(i.Name, cancellationToken));
			}

			return vmList;
		}

        public async Task PopulateCache(CancellationToken cancellationToken)
        {
			var pokemonList = await _dataClient.GetPokemonList(PokemonLimit, cancellationToken: cancellationToken);

			foreach (var i in pokemonList.Results)
			{
				await _dataClient.GetPokemonByName(i.Name);
			}
		}
	}
}
