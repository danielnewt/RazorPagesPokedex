using RazorPagesPokedex.PokemonApiClient.Models;

namespace RazorPagesPokedex.PokemonApiClient.Models
{
	public class Pokemon
    {
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public int Weight { get; set; }
		public int Height { get; set; }
		public PokemonSprites Sprites { get; set; } = new();
		public IEnumerable<PokemonType> Types { get; set; } = Array.Empty<PokemonType>();
	}
}
