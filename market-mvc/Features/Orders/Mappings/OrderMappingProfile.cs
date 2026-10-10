using AutoMapper;
using market_mvc.Domain.entity;
using market_mvc.Domain.ViewModels.Order;

namespace market_mvc.Features.Orders.Mappings
{
    /// <summary>
    /// AutoMapper profile for Order entity mappings
    /// </summary>
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            // Entity to ViewModel bidirectional mapping
            CreateMap<Order, CreateOrderVM>().ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.RowVersion, opt => opt.Ignore());
        }
    }
}

