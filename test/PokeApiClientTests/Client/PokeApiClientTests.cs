using FluentAssertions;
using PokeApiClient.Client;
using PokeApiClient.Client.Models;
using PokeApiClient.Client.Models.Pokemon;
using PokeApiClient.Options;
using System.Net;
using System.Text.Json;

namespace PokeApiClientTests.Client;

public class PokeApiClientTests
{
	private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
	private readonly PokeApiOptions _pokeApiOptions = new PokeApiOptions { PokeApiBaseUri = "https://test/" };

	[Fact]
	public async Task GetResourceById_Pokemon()
	{
		var response = HttpHelper.BuildResponse(HttpStatusCode.OK, HttpHelper.PokemonBulbasaurResponse);
		var (sut, messageHandler) = HttpHelper.GetPokeApiClientWithHandler(_pokeApiOptions, response);

		var id = Guid.NewGuid().ToString();
		var result = await sut.GetResourceById<Pokemon>(id);

		var expectedResult = JsonSerializer.Deserialize<Pokemon>(HttpHelper.PokemonBulbasaurResponse, _serializerOptions);
		result.Should().BeEquivalentTo(expectedResult);

		var expectedUrl = _pokeApiOptions.PokeApiBaseUri + $"pokemon/{id}/";
		messageHandler.RequestCountForUrl(expectedUrl).Should().Be(1);
		messageHandler.TotalRequestCount().Should().Be(1);

		//a second call should be cached which should result in the same result without any additional requests
		result = await sut.GetResourceById<Pokemon>(id);

		result.Should().BeEquivalentTo(expectedResult);

		messageHandler.RequestCountForUrl(expectedUrl).Should().Be(1);
		messageHandler.TotalRequestCount().Should().Be(1);
	}

	[Fact]
	public async Task GetResource_Pokemon_RelativeUrl()
	{
		var response = HttpHelper.BuildResponse(HttpStatusCode.OK, HttpHelper.PokemonBulbasaurResponse);
		var (sut, messageHandler) = HttpHelper.GetPokeApiClientWithHandler(_pokeApiOptions, response);

		var relativeUrl = $"pokemon/{Guid.NewGuid()}/";
		var result = await sut.GetResource<Pokemon>(relativeUrl);

		var expectedResult = JsonSerializer.Deserialize<Pokemon>(HttpHelper.PokemonBulbasaurResponse, _serializerOptions);
		result.Should().BeEquivalentTo(expectedResult);

		var expectedUrl = _pokeApiOptions.PokeApiBaseUri + relativeUrl;
		messageHandler.RequestCountForUrl(expectedUrl).Should().Be(1);
		messageHandler.TotalRequestCount().Should().Be(1);

		//a second call should be cached which should result in the same result without any additional requests
		result = await sut.GetResource<Pokemon>(relativeUrl);

		result.Should().BeEquivalentTo(expectedResult);

		messageHandler.RequestCountForUrl(expectedUrl).Should().Be(1);
		messageHandler.TotalRequestCount().Should().Be(1);
	}

	[Fact]
	public async Task GetResource_Pokemon_AbsoluteUrl()
	{
		var response = HttpHelper.BuildResponse(HttpStatusCode.OK, HttpHelper.PokemonBulbasaurResponse);
		var (sut, messageHandler) = HttpHelper.GetPokeApiClientWithHandler(_pokeApiOptions, response);

		var url = $"https://{Guid.NewGuid()}/pokemon/{Guid.NewGuid()}/";
		var result = await sut.GetResource<Pokemon>(url);

		var expectedResult = JsonSerializer.Deserialize<Pokemon>(HttpHelper.PokemonBulbasaurResponse, _serializerOptions);
		result.Should().BeEquivalentTo(expectedResult);

		messageHandler.RequestCountForUrl(url).Should().Be(1);
		messageHandler.TotalRequestCount().Should().Be(1);

		//a second call should be cached which should result in the same result without any additional requests
		result = await sut.GetResource<Pokemon>(url);

		result.Should().BeEquivalentTo(expectedResult);

		messageHandler.RequestCountForUrl(url).Should().Be(1);
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
		var result = await sut.GetResourceList<Pokemon>(limit, offset, includeResource);

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
		result = await sut.GetResourceList<Pokemon>(limit, offset, includeResource);

		result.Should().BeEquivalentTo(expectedResult);

		messageHandler.RequestCountForUrl(expectedUrl).Should().Be(1);
		messageHandler.TotalRequestCount().Should().Be(totalRequests);
	}

	[Fact]
	public async Task GetResource_UrlInvalid()
	{
		var response = HttpHelper.BuildResponse(HttpStatusCode.OK, HttpHelper.PokemonBulbasaurResponse);
		var (sut, messageHandler) = HttpHelper.GetPokeApiClientWithHandler(_pokeApiOptions, response);

		var url = "http://not a valid url";
		var ex = await Assert.ThrowsAsync<ApiResponseException>(() => sut.GetResource<Pokemon>(url));

		ex.Should().NotBeNull();
		var innerMessage = "Failed to create valid url from value: http://not a valid url (Parameter 'url')";
		ex.Message.Should().Be($"GET request failed due to unexpected error: {innerMessage}");
		ex.InnerException.Should().NotBeNull();
		ex.InnerException.Should().BeOfType<ArgumentException>();
		ex.InnerException!.Message.Should().Be(innerMessage);

		messageHandler.RequestCountForUrl(url).Should().Be(0);
		messageHandler.TotalRequestCount().Should().Be(0);
	}

	[Fact]
	public async Task GetResource_ResponeIsEmpty()
	{
		var response = HttpHelper.BuildResponse(HttpStatusCode.OK);
		var (sut, messageHandler) = HttpHelper.GetPokeApiClientWithHandler(_pokeApiOptions, response);

		var url = "https://test/";
		var ex = await Assert.ThrowsAsync<ApiResponseException>(() => sut.GetResource<Pokemon>(url));

		ex.Should().NotBeNull();
		ex.Message.Should().Be($"Unable to determine value for uri: {url}");
		ex.InnerException.Should().BeNull();

		messageHandler.RequestCountForUrl(url).Should().Be(1);
		messageHandler.TotalRequestCount().Should().Be(1);
	}

	[Fact]
	public async Task GetResource_ResponeIsUnsuccessful()
	{
		var statusCode = HttpStatusCode.BadRequest;
		var response = HttpHelper.BuildResponse(statusCode);
		var (sut, messageHandler) = HttpHelper.GetPokeApiClientWithHandler(_pokeApiOptions, response);

		var url = "https://test/";
		var ex = await Assert.ThrowsAsync<ApiResponseException>(() => sut.GetResource<Pokemon>(url));

		ex.Should().NotBeNull();
		ex.Message.Should().Be($"GET response contained unexpected status code: {statusCode}");
		ex.InnerException.Should().NotBeNull();
		ex.InnerException.Should().BeOfType<HttpRequestException>();
		ex.InnerException!.Message.Should().Be("Response status code does not indicate success: 400 (Bad Request).");

		messageHandler.RequestCountForUrl(url).Should().Be(1);
		messageHandler.TotalRequestCount().Should().Be(1);
	}
}
