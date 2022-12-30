using FluentAssertions;
using PokeApiClient.Client.Models;
using PokeApiClient.Client.Models.Pokemon;
using PokeApiClient.Extensions;
using PokeApiClient.Options;
using System.Net;
using System.Text.Json;

namespace PokeApiClientTests.Extensions;

public class PokeApiClientExtensionsTests
{
	private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
	private readonly PokeApiOptions _pokeApiOptions = new PokeApiOptions { PokeApiBaseUri = "https://test/" };

	[Fact]
	public async Task GetPokemon()
	{
		var response = HttpHelper.BuildResponse(HttpStatusCode.OK, HttpHelper.PokemonBulbasaurResponse);
		var (sut, messageHandler) = HttpHelper.GetPokeApiClientWithHandler(_pokeApiOptions, response);

		var id = Guid.NewGuid().ToString();
		var result = await sut.GetPokemon(id);

		var expectedResult = JsonSerializer.Deserialize<Pokemon>(HttpHelper.PokemonBulbasaurResponse, _serializerOptions);
		result.Should().BeEquivalentTo(expectedResult);

		var expectedUrl = _pokeApiOptions.PokeApiBaseUri + $"pokemon/{id}/";
		messageHandler.RequestCountForUrl(expectedUrl).Should().Be(1);
		messageHandler.TotalRequestCount().Should().Be(1);

		//a second call should be cached which should result in the same result without any additional requests
		result = await sut.GetPokemon(id);

		result.Should().BeEquivalentTo(expectedResult);

		messageHandler.RequestCountForUrl(expectedUrl).Should().Be(1);
		messageHandler.TotalRequestCount().Should().Be(1);
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public async Task GetResourceList_Pokemon(bool includeResource)
	{

		var (sut, messageHandler) = HttpHelper.GetPokeApiClientWithHandler(
			_pokeApiOptions,
			HttpHelper.BuildResponse(HttpStatusCode.OK, HttpHelper.ListPokemonResponseBody),
			HttpHelper.BuildResponse(HttpStatusCode.OK, HttpHelper.PokemonBulbasaurResponse));

		var limit = 10;
		var offset = 11;
		var result = await sut.GetPokemonList(limit, offset, includeResource);

		var totalRequests = 1;
		var expectedResult = JsonSerializer.Deserialize<ResourceList<Pokemon>>(HttpHelper.ListPokemonResponseBody, _serializerOptions);

		if (includeResource)
		{
			//we're expecting only one
			messageHandler.RequestCountForUrl(result.Results.Single().Url).Should().Be(1);
			expectedResult!.Results.Single().Resource = JsonSerializer.Deserialize<Pokemon>(HttpHelper.PokemonBulbasaurResponse, _serializerOptions);
			totalRequests++;
		}

		result.Should().BeEquivalentTo(expectedResult);

		var expectedUrl = _pokeApiOptions.PokeApiBaseUri + $"pokemon?limit={limit}&offset={offset}";
		messageHandler.RequestCountForUrl(expectedUrl).Should().Be(1);
		messageHandler.TotalRequestCount().Should().Be(totalRequests);



		//a second call should be cached which should result in the same result without any additional requests
		result = await sut.GetPokemonList(limit, offset, includeResource);

		result.Should().BeEquivalentTo(expectedResult);

		messageHandler.RequestCountForUrl(expectedUrl).Should().Be(1);
		messageHandler.TotalRequestCount().Should().Be(totalRequests);
	}
}
