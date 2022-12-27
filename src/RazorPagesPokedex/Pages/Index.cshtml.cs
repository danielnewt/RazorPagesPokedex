using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesPokedex.Models;
using RazorPagesPokedex.Services;

namespace RazerPagesPokedex.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IPokemonVmService _pokemonService;

        public IndexModel(ILogger<IndexModel> logger, IPokemonVmService pokemonService)
        {
            _logger = logger;
			_pokemonService = pokemonService;
        }

        [FromQuery(Name = "page")]
        public int CurrentPage { get; set; } = 1;

		public int PageCount => _pokemonService.PageCount;
		public int? NextPage => CurrentPage < PageCount ? CurrentPage + 1 : null;
		public int? PreviousPage => CurrentPage > 1 ? CurrentPage - 1 : null;

        public IEnumerable<PokemonVm> Pokemon { get; set; } = Array.Empty<PokemonVm>();

		public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (CurrentPage < 1 || CurrentPage > PageCount)
                    CurrentPage = 1;

                Pokemon = await _pokemonService.GetPokemonListPage(CurrentPage, cancellationToken);
			}
            catch(Exception e)
            {
                _logger.LogError(e, "Failed to populate PokemonListModel.Pokemon");
            }
        }
	}
}
