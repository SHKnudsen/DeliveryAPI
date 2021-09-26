using AutoMapper;
using DeliveryAPI.Core.Models;
using DeliveryAPI.UserService.DTOs;

namespace DeliveryAPI.UserService.Profiles
{
    public class OrderProfiles : Profile
    {
        public OrderProfiles()
        {
            CreateMap<Root, CreateOrderDto>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Order.OrderNumber))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.AccessWindow.StartTime));
        }
    }
}
