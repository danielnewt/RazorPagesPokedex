namespace PokeApiClient.Client.Models;

public class NamedApiResource<T> where T : BaseResource
{
	public string Name { get; set; } = string.Empty;
	public string Url { get; set; } = string.Empty;
	public T? Resource { get; set; }
}
