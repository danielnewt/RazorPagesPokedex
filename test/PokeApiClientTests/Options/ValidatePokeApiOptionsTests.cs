using FluentAssertions;
using Microsoft.Extensions.Options;
using PokeApiClient.Options;
using System.Collections;

namespace PokeApiClientTests.Options;

public class ValidatePokeApiOptionsTests
{
	private readonly IValidateOptions<PokeApiOptions> _sut;

	public ValidatePokeApiOptionsTests()
	{
		_sut = new ValidatePokeApiOptions();
	}

	[Fact]
	public void DefaultValuesAreValid()
	{
		var options = new PokeApiOptions();
		var result = _sut.Validate(null, options);
		result.Should().Be(ValidateOptionsResult.Success);
	}

	[Theory]
	[ClassData(typeof(InvalidOptionsTestCases))]
	public void InvalidOptionsReturnExpectedMessage(Action<PokeApiOptions> invalidAction, string expectedMessage)
	{
		var options = new PokeApiOptions();
		invalidAction(options);

		var result = _sut.Validate(null, options);
		result.Succeeded.Should().BeFalse();
		result.Failures.Should().ContainSingle();
		result.Failures.Single().Should().Be($"{nameof(ValidatePokeApiOptions)} Failure: {expectedMessage}");
	}

	public class InvalidOptionsTestCases : IEnumerable<object[]>
	{
		public IEnumerator<object[]> GetEnumerator() => Data().GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		private IEnumerable<object[]> Data()
		{
			/*
			 * Each test case contains two arguments:
			 * 1. The action which will make the options invalid
			 * 2. The expected error message from the validator
			 */

			//PokeApiBaseUri tests
			yield return new object[] {
				(Action<PokeApiOptions>)(x => x.PokeApiBaseUri = string.Empty),
				$"{nameof(PokeApiOptions.PokeApiBaseUri)} can not be null or empty"
			};

			yield return new object[] {
				(Action<PokeApiOptions>)(x => x.PokeApiBaseUri = " "),
				$"{nameof(PokeApiOptions.PokeApiBaseUri)} can not be null or empty"
			};

			yield return new object[] {
				(Action<PokeApiOptions>)(x => x.PokeApiBaseUri = "not a valid uri"),
				$"{nameof(PokeApiOptions.PokeApiBaseUri)} must be a valid absolute uri"
			};

			yield return new object[] {
				(Action<PokeApiOptions>)(x => x.PokeApiBaseUri = "notvalid.com"),
				$"{nameof(PokeApiOptions.PokeApiBaseUri)} must be a valid absolute uri"
			};

			yield return new object[] {
				(Action<PokeApiOptions>)(x => x.PokeApiBaseUri = "https://uri.com/missing/trailing/slash"),
				$"{nameof(PokeApiOptions.PokeApiBaseUri)} must end with a trailing forward slash"
			};

			//PokeApiTimeoutSeconds tests
			yield return new object[] {
				(Action<PokeApiOptions>)(x => x.PokeApiTimeoutSeconds = 0),
				$"{nameof(PokeApiOptions.PokeApiTimeoutSeconds)} must be greater than 0"
			};

			yield return new object[] {
				(Action<PokeApiOptions>)(x => x.PokeApiTimeoutSeconds = -1),
				$"{nameof(PokeApiOptions.PokeApiTimeoutSeconds)} must be greater than 0"
			};

			//PokeApiCachMinutes tests
			yield return new object[] {
				(Action<PokeApiOptions>)(x => x.PokeApiCacheMinutes = 0),
				$"{nameof(PokeApiOptions.PokeApiCacheMinutes)} must be greater than 0"
			};

			yield return new object[] {
				(Action<PokeApiOptions>)(x => x.PokeApiCacheMinutes = -1),
				$"{nameof(PokeApiOptions.PokeApiCacheMinutes)} must be greater than 0"
			};
		}
	}
}
