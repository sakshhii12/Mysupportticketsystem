using AutoMapper;
using MysupportticketsystemBackend.Models;
using MysupportticketsystemBackend.Models.DTOs;

namespace MysupportticketsystemBackend.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
           
            CreateMap<Ticket, TicketDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email));

            
            CreateMap<CreateTicketDto, Ticket>()
               
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => Enum.Parse<TicketPriority>(src.Priority, true)))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => Enum.Parse<TicketCategory>(src.Category, true)));

            CreateMap<UpdateTicketDto, Ticket>()
    
    .ForMember(dest => dest.Title, opt => {
        opt.Condition(src => src.Title != null);
        opt.MapFrom(src => src.Title);
    })
    .ForMember(dest => dest.Description, opt => {
        opt.Condition(src => src.Description != null);
        opt.MapFrom(src => src.Description);
    })
    .ForMember(dest => dest.Priority, opt => {
        opt.Condition(src => src.Priority != null);
        opt.MapFrom(src => Enum.Parse<TicketPriority>(src.Priority, true));
    })
    .ForMember(dest => dest.Category, opt => {
        opt.Condition(src => src.Category != null);
        opt.MapFrom(src => Enum.Parse<TicketCategory>(src.Category, true));
    })
    .ForMember(dest => dest.Status, opt => {
        opt.Condition(src => src.Status != null);
        opt.MapFrom(src => Enum.Parse<TicketStatus>(src.Status, true));
    });
        }
    }
}