using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesPokedex.Models;
using RazorPagesPokedex.Requests.Queries;

namespace RazerPagesPokedex.Pages
{
	public class IndexModel : PageModel
	{
		private readonly ILogger<IndexModel> _logger;
		private readonly IMediator _mediator;

		public IndexModel(ILogger<IndexModel> logger, IMediator mediator)
		{
			_logger = logger;
			_mediator = mediator;
		}

		public const int PageSize = 6;
		public const int PokemonLimit = 151;

		[FromQuery(Name = "page")]
		public int CurrentPage { get; set; } = 1;

		public int PageCount => PageSize > 0 ? (int)Math.Ceiling((double)PokemonLimit / PageSize) : 0;
		public int? NextPage => CurrentPage < PageCount ? CurrentPage + 1 : null;
		public int? PreviousPage => CurrentPage > 1 ? CurrentPage - 1 : null;

		public IEnumerable<PokemonVm> Pokemon { get; set; } = Array.Empty<PokemonVm>();

		public async Task OnGetAsync(CancellationToken cancellationToken)
		{
			try
			{
				if (CurrentPage < 1 || CurrentPage > PageCount)
					CurrentPage = 1;

				Pokemon = await _mediator.Send(new GetPokemonVmList(CurrentPage, PageSize, PokemonLimit), cancellationToken);
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Failed to populate PokemonListModel.Pokemon");
			}
		}
	}
}
