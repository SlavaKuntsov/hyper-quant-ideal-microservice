using AutoMapper;
using InfoSymbolServer.Application.Dtos;
using InfoSymbolServer.Presentation.Requests;

namespace InfoSymbolServer.Presentation.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateExchangeRequest, CreateExchangeDto>();
        CreateMap<UpdateNotificationSettingsRequest, UpdateNotificationSettingsDto>();
        CreateMap<AddSymbolRequest, AddSymbolDto>();
    }
}
