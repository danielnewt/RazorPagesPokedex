using Microsoft.Extensions.Options;

namespace PokeApiClient.Options
{
    public class ValidatePokeApiOptions : IValidateOptions<PokeApiOptions>
    {
        public ValidateOptionsResult Validate(string? name, PokeApiOptions options)
        {
            foreach (var v in ValidationRules)
            {
                var failure = v(options);

                if (!string.IsNullOrWhiteSpace(failure))
                    return ValidateOptionsResult.Fail($"{nameof(ValidatePokeApiOptions)} Failure: {failure}");
            }

            return ValidateOptionsResult.Success;
        }

        private Func<PokeApiOptions, string>[] ValidationRules => new[]{
            ValidateNotNull,
            ValidatePokeApiBaseUri,
            ValidatePokeApiTimeoutSeconds,
            ValidatePokeApiCacheMinutes
        };

        private string ValidateNotNull(PokeApiOptions options)
        {
            if (options == null)
                return "Options is null";

            return string.Empty;
        }

        private string ValidatePokeApiBaseUri(PokeApiOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.PokeApiBaseUri))
                return $"{nameof(options.PokeApiBaseUri)} can not be null or empty";

            if (!Uri.TryCreate(options.PokeApiBaseUri, UriKind.Absolute, out var uri))
                return $"{nameof(options.PokeApiBaseUri)} must be a valid absolute uri";

            if (!uri.ToString().EndsWith("/"))
                return $"{nameof(options.PokeApiBaseUri)} must end with a trailing forward slash";

            return string.Empty;
        }

        private string ValidatePokeApiTimeoutSeconds(PokeApiOptions options)
        {
            if (options.PokeApiTimeoutSeconds <= 0)
                return $"{nameof(options.PokeApiTimeoutSeconds)} must be greater than 0";

            return string.Empty;
        }

        private string ValidatePokeApiCacheMinutes(PokeApiOptions options)
        {
            if (options.PokeApiCacheMinutes <= 0)
                return $"{nameof(options.PokeApiCacheMinutes)} must be greater than 0";

            return string.Empty;
        }
    }
}
