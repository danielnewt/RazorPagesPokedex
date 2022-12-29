namespace PokeApiClient.Client.Models;

public class ResourceList<T> : BaseResource where T : BaseResource
{
	public int Count { get; set; }
	public string Next { get; set; } = string.Empty;
	public string Previous { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public IEnumerable<NamedApiResource<T>> Results { get; set; } = Array.Empty<NamedApiResource<T>>();
}
