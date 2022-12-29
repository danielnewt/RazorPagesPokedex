using Microsoft.Extensions.Options;

namespace PokeApiClient.Options
{
    public class PokeApiOptions
	{
        public string PokeApiBaseUri { get; set; } = "https://pokeapi.co/api/v2/";
        public int PokeApiTimeoutSeconds { get; set; } = 30;
        public int PokeApiCacheMinutes { get; set; } = 60;
	}
}
