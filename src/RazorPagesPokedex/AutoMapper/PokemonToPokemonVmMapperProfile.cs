using AutoMapper;
using PokeApiClient.Client.Models.Pokemon;
using RazorPagesPokedex.Models;

namespace RazorPagesPokedex.Mapping
{
	public class PokemonToPokemonVmMapperProfile : Profile
    {
        public PokemonToPokemonVmMapperProfile()
        {
			CreateMap<Pokemon, PokemonVm>()
			.ForMember(dest =>
				dest.SpriteImage,
				opt => opt.MapFrom(src => src.Sprites.FrontDefault))
			.ForMember(dest =>
				dest.Types,
				opt => opt.MapFrom(src => src.Types.Select(x => x.Type.Name).ToArray()));
		}
    }
}
