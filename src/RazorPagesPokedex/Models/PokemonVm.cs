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
    }
}
