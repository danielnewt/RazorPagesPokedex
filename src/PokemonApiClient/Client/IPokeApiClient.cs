using PokeApiClient.Client.Models;
using PokeApiClient.Client.Models.Pokemon;

namespace PokeApiClient.Client
{
    public interface IPokeApiClient
	{
		/// <summary>
		/// Gets the resource requested by the url and deserializes the response body as type T.
		/// Responses are cached against the uri of the request.
		/// </summary>
		/// <typeparam name="T">The type to deserialize the response as.</typeparam>
		/// <param name="url">Absolute or Relative path to the resource. Relative path must work with BaseAddress of the HttpClient.</param>
		/// <param name="cancellationToken">Cancellation Token.</param>
		/// <returns>The response from the api deserialized as type T.</returns>
		Task<T> GetResource<T>(string url, CancellationToken cancellationToken = default) where T : BaseResource;

		/// <summary>
		/// Requests the resource from the API by the id provided and deserializes it into a type T.
		/// </summary>
		/// <typeparam name="T">The resource type.</typeparam>
		/// <param name="id">The resource identifier value.</param>
		/// <param name="cancellationToken">Cancellation Token.</param>
		/// <returns>The response from the api deserialized as type T.</returns>
		Task<T> GetResourceById<T>(string id, CancellationToken cancellationToken = default) where T : BaseResource;

		/// <summary>
		/// Requests the resource list from the API and deserializes it into a ResourceList<T>.
		/// </summary>
		/// <typeparam name="T">The resource type.</typeparam>
		/// <param name="limit">The limit parameter value.</param>
		/// <param name="offset">The offset parameter value.</param>
		/// <param name="includeResource">If true the Resource property of the NamedResource<T> will be populated by making subsequent calls.</param>
		/// <param name="cancellationToken">Cancellation Token.</param>
		/// <returns>The list response from the Api deserialized as a ResourceList<T>.</returns>
		Task<ResourceList<T>> GetResourceList<T>(int limit = 20, int offset = 0, bool includeResource = false, CancellationToken cancellationToken = default) where T : BaseResource;
	}
}
