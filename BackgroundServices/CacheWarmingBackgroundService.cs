using RazerPagesPokedex;
using RazorPagesPokedex.Services;

namespace RazorPagesPokedex.BackgroundServices
{
    public class CacheWarmingBackgroundService : BackgroundService
    {
        private readonly ILogger<CacheWarmingBackgroundService> _logger;
        private readonly ServiceConfig _config;
        private readonly IPokemonVmService _pokemonService;

        public CacheWarmingBackgroundService(
            ILogger<CacheWarmingBackgroundService> logger,
            ServiceConfig config,
			IPokemonVmService pokemonService)
        {
            _logger = logger;
            _config = config;
			_pokemonService = pokemonService;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_config.CacheWarmingEnabled)
            {
                _logger.LogDebug("{service} is enabled", GetType().Name);
            }
            else
            {
				_logger.LogDebug("{service} is disabled", GetType().Name);
                return;
			}

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _pokemonService.PopulateCache(stoppingToken);
                    await Task.Delay(TimeSpan.FromMinutes(_config.PokemonCacheMinutes * 0.5), stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to warm cache");
                }
            }
        }
    }
}
