namespace RazorPagesPokedex.PokemonApiClient.Models
{
	public class PokemonList
	{
		public IEnumerable<PokemonListItem> Results { get; set; } = Array.Empty<PokemonListItem>();
	}
}
