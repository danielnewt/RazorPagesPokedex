namespace PokeApiClient.Client;

public class ApiResponseException : Exception
{
	public ApiResponseException(string? message = null, Exception? inner = null) : base(message, inner)
	{
	}
}
