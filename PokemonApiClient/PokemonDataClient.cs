using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using RazorPagesPokedex.Extensions;
using RazorPagesPokedex.PokemonApiClient;
using RazorPagesPokedex.PokemonApiClient.Models;
using System.Text.Json;

namespace RazerPagesPokedex.PokemonApiClient
{
	public class PokemonDataClient : IPokemonDataClient
	{
		private readonly HttpClient _httpClient;
		private readonly IDistributedCache _distributedCache;
		private readonly DistributedCacheEntryOptions _cacheOptions;
		private readonly JsonSerializerOptions _serializerOptions;
		private readonly string _pokemonEndpoint;

		public PokemonDataClient(
			HttpClient httpClient,
			IDistributedCache distributedCache,
			ServiceConfig config,
			JsonSerializerOptions serializerOptions)
		{
			_httpClient = httpClient;
			_distributedCache = distributedCache;
			_serializerOptions = serializerOptions;

			_httpClient.BaseAddress = new Uri(config.PokemonApiUri, UriKind.Absolute);
			_pokemonEndpoint = config.PokemonApiPokemonEndpoint.Trim('/');
			_httpClient.Timeout = TimeSpan.FromSeconds(config.PokemonApiTimeoutSeconds);

			_cacheOptions = new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(config.PokemonCacheMinutes)
			};
		}

		public async Task<PokemonList> GetPokemonList(int limit = 0, int offset = 0, CancellationToken cancellationToken = default)
		{
			string query = $"?limit={limit}&offset={offset}";

			try
			{
				return await _distributedCache.GetItemAsync(
								$"PokemonApi.Get.Pokemon{query}",
								() => _httpClient.GetFromJsonAsync<PokemonList>($"{_pokemonEndpoint}/{query}", _serializerOptions, cancellationToken),
								_serializerOptions,
								_cacheOptions,
								cancellationToken) ??
								 throw PokemonApiException.GetPokemonListFailure();
			}
			catch (Exception e)
			{
				throw PokemonApiException.GetPokemonListFailure(e);
			}
		}

		public async Task<Pokemon> GetPokemonById(int id, CancellationToken cancellationToken = default)
		{
			try
			{
				return await _distributedCache.GetItemAsync(
				$"PokemonApi.Get.Pokemon.{id}",
				() => _httpClient.GetFromJsonAsync<Pokemon>($"{_pokemonEndpoint}/{id}", _serializerOptions, cancellationToken),
				_serializerOptions,
				_cacheOptions,
				cancellationToken) ??
				 throw PokemonApiException.GetPokemonByIdFailure(id);
			}
			catch (Exception e)
			{
				throw PokemonApiException.GetPokemonByIdFailure(id, e);
			}
		}

		public async Task<Pokemon> GetPokemonByName(string name, CancellationToken cancellationToken = default)
		{
			try
			{
				return await _distributedCache.GetItemAsync(
				$"PokemonApi.Get.Pokemon.{name}",
				() => _httpClient.GetFromJsonAsync<Pokemon>($"{_pokemonEndpoint}/{name}", _serializerOptions, cancellationToken),
				_serializerOptions,
				_cacheOptions,
				cancellationToken) ??
				throw PokemonApiException.GetPokemonByNameFailure(name);
			}
			catch (Exception e)
			{
				throw PokemonApiException.GetPokemonByNameFailure(name, e);
			}
		}
	}
}
