namespace PokeApiClient.Client.Models.Pokemon
{
	public class Pokemon : BaseResource
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public int Weight { get; set; }
		public int Height { get; set; }
		public PokemonSprites Sprites { get; set; } = new();
		public IEnumerable<PokemonType> Types { get; set; } = Array.Empty<PokemonType>();

	}
}
