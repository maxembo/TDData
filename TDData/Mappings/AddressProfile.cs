using AutoMapper;
using TDData.DTO;

namespace TDData.Mappings;

public class AddressProfile : Profile
{
    public AddressProfile()
    {
        CreateMap<AddressResponseDto, AddressDto>();
    }
}