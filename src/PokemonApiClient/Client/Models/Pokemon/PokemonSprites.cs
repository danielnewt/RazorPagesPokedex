using System.Text.Json.Serialization;

namespace PokeApiClient.Client.Models.Pokemon
{
	public class PokemonSprites
	{
		[JsonPropertyName("front_default")]
		public string FrontDefault { get; set; } = string.Empty;
	}
}
