namespace PokeApiClient
{
	public class PokeApiOptions
	{
		public string PokemonApiUri { get; set; } = "https://pokeapi.co/api/v2/";
		public int PokeApiTimeoutSeconds { get; set; } = 30;
		public int PokeApiCacheMinutes { get; set; } = 60;
	}
}
