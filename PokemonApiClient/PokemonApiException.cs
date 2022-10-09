namespace RazorPagesPokedex.PokemonApiClient
{
	public class PokemonApiException : Exception
	{
		public PokemonApiException(string? message = null, Exception? inner = null) : base(message, inner)
		{

		}

		internal static PokemonApiException GetPokemonListFailure(Exception? inner = null)
		{
			return new PokemonApiException($"Failed to Get Pokemon List", inner);
		}

		internal static PokemonApiException GetPokemonByIdFailure(int id, Exception? inner = null)
		{
			return new PokemonApiException($"Failed to Get Pokemon By Id - {id}", inner);
		}

		internal static PokemonApiException GetPokemonByNameFailure(string name, Exception? inner = null)
		{
			return new PokemonApiException($"Failed to Get Pokemon By Name - {name}", inner);
		}
	}
}
