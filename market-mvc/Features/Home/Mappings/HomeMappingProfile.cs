using AutoMapper;
using market_mvc.Models.entity;
using market_mvc.Models.ViewModels.Product;

namespace market_mvc.Features.Home.Mappings
{
    /// <summary>
    /// AutoMapper profile for Home feature mappings
    /// </summary>
    public class HomeMappingProfile : Profile
    {
        public HomeMappingProfile()
        {
            // Entity to ViewModel mappings for home page
            CreateMap<Product, ProductCardVM>();
        }
    }
}
