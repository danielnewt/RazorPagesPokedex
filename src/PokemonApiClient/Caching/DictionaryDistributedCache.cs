using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PokeApiClient.Caching
{
	internal class DictionaryDistributedCache : IDistributedCache
	{
		private readonly ConcurrentDictionary<string, byte[]> _cache = new ();

		public byte[]? Get(string key)
		{
			return _cache.TryGetValue(key, out var value) ? value : null;
		}

		public Task<byte[]?> GetAsync(string key, CancellationToken token = default)
		{
			return Task.FromResult(Get(key));
		}

		public void Refresh(string key)
		{
			//not relevent for dictionary implementation
		}

		public Task RefreshAsync(string key, CancellationToken token = default)
		{
			//not relevent for dictionary implementation
			return Task.CompletedTask;
		}

		public void Remove(string key)
		{
			_ = _cache.Remove(key, out _);
		}

		public Task RemoveAsync(string key, CancellationToken token = default)
		{
			Remove(key);
			return Task.CompletedTask;
		}

		public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
		{
			_cache[key] = value;
		}

		public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
		{
			Set(key, value, options);
			return Task.CompletedTask;
		}
	}
}
