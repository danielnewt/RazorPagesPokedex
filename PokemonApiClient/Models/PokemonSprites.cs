using System.Text.Json.Serialization;

namespace RazorPagesPokedex.PokemonApiClient.Models
{
	public class PokemonSprites
	{
		[JsonPropertyName("front_default")]
		public string FrontDefault { get; set; } = string.Empty;
	}
}
