using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PokeApiClient.Client;
using PokeApiClient.DependencyInjection;
using PokeApiClient.Options;

namespace PokeApiClientTests.DependencyInjection;
public class ServiceCollectionExtensionsTests
{
	[Fact]
	public void AddPokeApiClient()
	{
		var services = new ServiceCollection();
		services.AddPokeApiClient(x => { });

		var sp = services.BuildServiceProvider();

		sp.GetRequiredService<IPokeApiClient>().Should().NotBeNull();
		sp.GetRequiredService<IOptions<PokeApiOptions>>().Should().NotBeNull();
		sp.GetRequiredService<IValidateOptions<PokeApiOptions>>().Should().NotBeNull();
	}

	[Fact]
	public void AddPokeApiClient_OptionsValidationFails()
	{
		var services = new ServiceCollection();
		services.AddPokeApiClient(x => x.PokeApiCacheMinutes = -1); //should be invalid
		var sp = services.BuildServiceProvider();

		var ex = Assert.Throws<OptionsValidationException>(() => sp.GetRequiredService<IPokeApiClient>());
		ex.Should().NotBeNull();
		ex.Message.Should().Be("ValidatePokeApiOptions Failure: PokeApiCacheMinutes must be greater than 0");
	}
}
