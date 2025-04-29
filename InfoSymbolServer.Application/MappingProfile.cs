using AutoMapper;
using InfoSymbolServer.Application.Dtos;
using InfoSymbolServer.Domain.Models;

namespace InfoSymbolServer.Application;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Exchange mappings
        CreateMap<Exchange, ExchangeDto>();
        CreateMap<CreateExchangeDto, Exchange>();
        
        // Symbol mappings
        CreateMap<Symbol, SymbolDto>();
        CreateMap<AddSymbolDto, Symbol>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        
        // Status mappings
        CreateMap<Status, StatusDto>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.CreatedAt));
            
        // NotificationSettings mappings
        CreateMap<NotificationSettings, NotificationSettingsDto>();
        CreateMap<UpdateNotificationSettingsDto, NotificationSettings>();
    }
}
