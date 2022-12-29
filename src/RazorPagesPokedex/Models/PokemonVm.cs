using PokeApiClient.Client.Models.Pokemon;

namespace RazorPagesPokedex.Models
{
	public class PokemonVm
    {
        /*
         * Possible additions:
        Encounter locations
         */
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
		public int Weight { get; set; }
		public int Height { get; set; }
		public string SpriteImage { get; set; } = string.Empty;
		public IEnumerable<string> Types { get; set; } = Array.Empty<string>();

		public PokemonVm()
		{

		}

		public PokemonVm(Pokemon pokemon)
		{
			Height = pokemon.Height;
			Weight = pokemon.Weight;
			Id = pokemon.Id;
			Name = pokemon.Name;
			SpriteImage = pokemon.Sprites.FrontDefault;
			Types = pokemon.Types.Select(x => x.Type.Name);
		}
	}
}
