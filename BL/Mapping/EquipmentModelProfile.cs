using AutoMapper;
using Contracts.Equipment;

namespace BL.Mapping;

public class EquipmentModelProfile : Profile
{
    public EquipmentModelProfile()
    {
        CreateMap<EquipmentParameters, EquipmentModel>()
            .ForMember(x => x.EntityId, o => o.Ignore())
            .ForMember(x => x.Version, o => o.Ignore())
            .ForMember(x => x.Timestamp, o => o.Ignore())
            .ForMember(x => x.ModelVersion, o => o.Ignore());
    }
}
