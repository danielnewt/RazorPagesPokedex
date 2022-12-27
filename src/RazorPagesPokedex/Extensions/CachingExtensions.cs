using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace RazorPagesPokedex.Extensions
{
	public static class CachingExtensions
	{
		public static async Task<T?> GetItemAsync<T>(
			this IDistributedCache cache,
			string cacheKey,
			JsonSerializerOptions? jsonOptions = null,
			CancellationToken cancellationToken = default)
		{
			var cacheResult = await cache.GetStringAsync(cacheKey, cancellationToken);

			if (cacheResult == null)
				return default; //no cached value

			return JsonSerializer.Deserialize<T>(cacheResult, jsonOptions);
		}

		public static async Task<T?> GetItemAsync<T>(
			this IDistributedCache cache,
			string cacheKey,
			Func<Task<T?>> GetFunc,
			JsonSerializerOptions? jsonOptions = null,
			DistributedCacheEntryOptions? cacheOptions = null,
			CancellationToken cancellationToken = default)
		{
			var item = await cache.GetItemAsync<T>(cacheKey, jsonOptions, cancellationToken);

			if (item != null)
				return item; //cached item found

			item = await GetFunc();

			if (item == null)
				return item; //item is null, no need to set cache

			await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(item, jsonOptions), cacheOptions, cancellationToken);
			
			return item;
		}
	}
}
