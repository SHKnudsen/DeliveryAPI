using AutoMapper;
using DeliveryAPI.Core.Models;
using DeliveryAPI.PartnerService.DTOs;

namespace DeliveryAPI.PartnerService.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Root, GetOrderDto>();
            CreateMap<Root, OrderCompletedDto>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Order.OrderNumber))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State));

            CreateMap<Root, OrderExpiredDto>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Order.OrderNumber))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Recipient));
        }
    }
}
