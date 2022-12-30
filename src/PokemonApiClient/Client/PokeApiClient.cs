using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using PokeApiClient.Caching;
using PokeApiClient.Client.Models;
using PokeApiClient.Options;
using System.Text.Json;

namespace PokeApiClient.Client;

public class PokeApiClient : IPokeApiClient
{
	private readonly HttpClient _httpClient;
	private readonly IDistributedCache _distributedCache;
	private readonly DistributedCacheEntryOptions _cacheOptions;
	private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

	public PokeApiClient(
		HttpClient httpClient,
		IDistributedCache? distributedCache = null,
		IOptions<PokeApiOptions>? options = null)
	{
		_httpClient = httpClient;
		_distributedCache = distributedCache ?? new DictionaryDistributedCache();

		var config = options?.Value ?? new PokeApiOptions();
		if (config.PokeApiBaseUri != null)
			_httpClient.BaseAddress = new Uri(config.PokeApiBaseUri, UriKind.Absolute);
		_httpClient.Timeout = TimeSpan.FromSeconds(config.PokeApiTimeoutSeconds);

		_cacheOptions = new DistributedCacheEntryOptions
		{
			SlidingExpiration = TimeSpan.FromMinutes(config.PokeApiCacheMinutes)
		};
	}

	/// <inheritdoc />
	public async Task<ResourceList<T>> GetResourceList<T>(int limit = 20, int offset = 0, bool includeResource = false, CancellationToken cancellationToken = default) where T : BaseResource
	{
		var path = $"{GetPathForResource<T>()}?limit={limit}&offset={offset}";

		var resourceList = await GetResource<ResourceList<T>>(path, cancellationToken);

		if (!includeResource)
			return resourceList;

		foreach (var r in resourceList.Results)
		{
			r.Resource = await GetResource<T>(r.Url, cancellationToken);
		}

		return resourceList;
	}

	/// <inheritdoc />
	/// <exception cref="ArgumentException">Will be thrown if the id provided is null or whitespace.</exception>
	public async Task<T> GetResourceById<T>(string id, CancellationToken cancellationToken = default) where T : BaseResource
	{
		if (string.IsNullOrWhiteSpace(id))
			throw new ArgumentNullException(nameof(id));

		var path = $"{GetPathForResource<T>()}/{id}/";
		return await GetResource<T>(path, cancellationToken);
	}

	/// <inheritdoc />
	public async Task<T> GetResource<T>(string url, CancellationToken cancellationToken = default) where T : BaseResource
	{
		try
		{
			if (!Uri.TryCreate(url, UriKind.Absolute, out var absoluteUri) &&
				!Uri.TryCreate(_httpClient.BaseAddress, url, out absoluteUri))
				throw new ArgumentException($"Failed to create valid url from value: {url}", nameof(url));

			var cacheKey = absoluteUri.ToString();
			var stringValue = string.Empty;

			var cacheResult = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);
			if (cacheResult != null)
				stringValue = cacheResult;

			if (string.IsNullOrWhiteSpace(stringValue))
			{
				using var response = await _httpClient.GetAsync(absoluteUri, cancellationToken);

				response.EnsureSuccessStatusCode();

				stringValue = await response.Content.ReadAsStringAsync(cancellationToken);
			}

			if (string.IsNullOrWhiteSpace(stringValue))
				throw new ApiResponseException($"Unable to determine value for uri: {absoluteUri}");

			await _distributedCache.SetStringAsync(cacheKey, stringValue, _cacheOptions, cancellationToken);

			var deserializedResult = JsonSerializer.Deserialize<T>(stringValue, _serializerOptions);

			return deserializedResult ?? throw new ApiResponseException($"Unable to parse value for query: {absoluteUri}");
		}
		catch (ApiResponseException)
		{
			throw;
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

	/// <summary>
	/// Converts the type of T to the path required by the API.
	/// For most cases this will only require converting the name of the type to lowercase.
	///	Other cases will need to be added to the switch expression in this method body.
	/// </summary>
	/// <typeparam name="T">The type to get the path for.</typeparam>
	/// <returns>The string path matching the resource.</returns>
	private static string GetPathForResource<T>() where T : BaseResource =>
		typeof(T).Name switch
		{
			_ => typeof(T).Name.ToLowerInvariant()
		};
}
