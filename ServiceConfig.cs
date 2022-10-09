namespace RazerPagesPokedex
{
	public class ServiceConfig
	{
		public string PokemonApiUri { get; set; } = "https://pokeapi.co";
		public string PokemonApiPokemonEndpoint { get; set; } = "api/v2/pokemon";
		public int PokemonApiTimeoutSeconds { get; set; } = 30;
		public int PokemonCacheMinutes { get; set; } = 60 * 4;
		public bool CacheWarmingEnabled { get; set; } = false;
	}
}
