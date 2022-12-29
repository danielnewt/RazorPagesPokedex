using MediatR;
using PokeApiClient.Client;
using PokeApiClient.Extensions;
using RazorPagesPokedex.Models;

namespace RazorPagesPokedex.Requests.Queries;

public class GetPokemonVmList : IRequest<IEnumerable<PokemonVm>>
{
	public int PageSize { get; }
	public int Page { get; }
	public int PokemonLimit { get; }


	public GetPokemonVmList(
		int page = 1,
		int pageSize = 6,
		int pokemonLimit = 151)
	{
		if (page < 1)
			throw new ArgumentException("page must be greater than 0", nameof(page));

		if (pageSize < 1)
			throw new ArgumentException("pageSize must be greater than 0", nameof(pageSize));

		if (pokemonLimit < 1)
			throw new ArgumentException("pokemonLimit must be greater than 0", nameof(pokemonLimit));

		Page = page;
		PageSize = pageSize;
		PokemonLimit = pokemonLimit;
	}
}

public class GetPokemonVmListHandler : IRequestHandler<GetPokemonVmList, IEnumerable<PokemonVm>>
{
	private readonly ILogger<GetPokemonVmListHandler> _logger;
	private readonly IPokeApiClient _client;

	public GetPokemonVmListHandler(
		ILogger<GetPokemonVmListHandler> logger,
		IPokeApiClient client)
	{
		_logger = logger;
		_client = client;
	}

	public async Task<IEnumerable<PokemonVm>> Handle(GetPokemonVmList request, CancellationToken cancellationToken)
	{
		var offset = (request.Page - 1) * request.PageSize;
		var limit = Math.Min(request.PageSize, request.PokemonLimit - offset);

		var pokemonList = await _client.GetPokemonList(limit, offset, true, cancellationToken);

		return pokemonList.Results
			.Where(x => x.Resource != null)
			.Select(x => x.Resource)
			.Select(x => new PokemonVm(x!))
			.ToArray();
	}
}