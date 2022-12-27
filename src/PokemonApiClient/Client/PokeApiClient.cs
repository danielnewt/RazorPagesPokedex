using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using PokeApiClient.Caching;
using PokeApiClient.Client.Models;
using PokeApiClient.Client.Models.Pokemon;
using System.Text.Json;

namespace PokeApiClient.Client
{
	public class PokeApiClient : IPokeApiClient
	{
		private readonly HttpClient _httpClient;
		private readonly IDistributedCache? _distributedCache;
		private readonly DistributedCacheEntryOptions _cacheOptions;
		private readonly JsonSerializerOptions _serializerOptions;

		public PokeApiClient(
			HttpClient httpClient,
			IDistributedCache? distributedCache,
			IOptions<PokeApiOptions>? options,
			JsonSerializerOptions? serializerOptions)
		{
			_httpClient = httpClient;
			_distributedCache = distributedCache ?? new DictionaryDistributedCache();
			_serializerOptions = serializerOptions ?? new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

			var config = options?.Value ?? new PokeApiOptions();
			_httpClient.BaseAddress = new Uri(config.PokemonApiUri, UriKind.Absolute);
			_httpClient.Timeout = TimeSpan.FromSeconds(config.PokeApiTimeoutSeconds);

			_cacheOptions = new DistributedCacheEntryOptions
			{
				SlidingExpiration = TimeSpan.FromMinutes(config.PokeApiCacheMinutes)
			};
		}

		public async Task<Pokemon> GetPokemon(string id, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrWhiteSpace(id))
				throw new ArgumentNullException(nameof(id));

			return await GetResponse<Pokemon>($"pokemon/{id}", cancellationToken);
		}

		public async Task<ResourceList<Pokemon>> GetPokemonList(int limit = 0, int offset = 0, CancellationToken cancellationToken = default)
		{
			var list = await GetResponse<ResourceList<Pokemon>>($"pokemon?limit={limit}&offset={offset}", cancellationToken);

			foreach (var p in list.Results)
			{
				p.Resource = await GetPokemon(p.Name, cancellationToken);
			}

			return list;
		}

		private async Task<T> GetResponse<T>(string query, CancellationToken cancellationToken = default) where T : BaseResource
		{
			try
			{
				var stringValue = string.Empty;

				var cacheResult = await _distributedCache.GetStringAsync(query, cancellationToken);
				if (cacheResult != null)
					stringValue = cacheResult;

				if (string.IsNullOrWhiteSpace(stringValue))
				{
					var response = await _httpClient.GetAsync(query, cancellationToken);

					response.EnsureSuccessStatusCode();

					stringValue = await response.Content.ReadAsStringAsync();
				}

				if (string.IsNullOrWhiteSpace(stringValue))
					throw new ApiResponseException($"Unable to determine value for query: {query}");

				var deserializedResult = JsonSerializer.Deserialize<T>(stringValue, _serializerOptions);

				if (deserializedResult != null)
					await _distributedCache.SetStringAsync(query, stringValue, _cacheOptions, cancellationToken);

				return deserializedResult ?? throw new ApiResponseException($"Unable to parse value for query: {query}");
			}
			catch (HttpRequestException e)
			{
				throw new ApiResponseException($"GET response contained unexpected status code: {e.StatusCode}", e);
			}
			catch (Exception e)
			{
				throw new ApiResponseException($"GET request failed due to unexpected error: {e.Message}", e);
			}
		}

	}
}
