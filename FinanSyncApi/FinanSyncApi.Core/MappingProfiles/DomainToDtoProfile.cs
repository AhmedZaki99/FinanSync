using AutoMapper;
using FinanSyncApi.Data;
using FinanSyncData;

namespace FinanSyncApi.Core;

public sealed class DomainToDtoProfile : Profile
{

    public DomainToDtoProfile()
    {
        // User.
        CreateMap<AppUser, UserResponseDto>();


        // Settings.
        CreateMap<Setting, SettingResponseDto>()
            .ForMember(dto => dto.Value, config => config.MapFrom(model => model.DefaultValue));

        CreateMap<UserSetting, SettingResponseDto>()
            .ForMember(dto => dto.Name, config => config.MapFrom(model => model.Setting != null ? model.Setting.Name : null))
            .ForMember(dto => dto.ClrTypeCode, config => config.MapFrom(model => model.Setting != null ? model.Setting.ClrTypeCode : TypeCode.String))
            .ForMember(dto => dto.DefaultValue, config => config.MapFrom(model => model.Setting != null ? model.Setting.DefaultValue : null));
    }

}
