using Microsoft.Extensions.Options;

namespace RazorPagesPokedex.Extensions;

public static class WebApplicationBuilderExtensions
{
	public static void Configure<T>(this WebApplicationBuilder builder) where T : class
	{
		builder.Services.Configure<T>(x => builder.Configuration.GetSection(typeof(T).Name).Bind(x));
		builder.Services.AddTransient(x => x.GetRequiredService<IOptions<T>>().Value);
	}
}
