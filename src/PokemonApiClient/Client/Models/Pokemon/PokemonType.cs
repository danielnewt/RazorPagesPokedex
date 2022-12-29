namespace PokeApiClient.Client.Models.Pokemon
{
	public class PokemonType
	{
		public NamedApiResource<Type> Type { get; set; } = new();
	}
}
