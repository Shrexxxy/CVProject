using AutoMapper;
using Contracts.Equipment;
using BL.Entities;

namespace BL.Mapping;

public class EquipmentFilterProfile : Profile
{
    public EquipmentFilterProfile()
    {
        CreateMap<EquipmentFilterParameters, EquipmentFilterSettings>()
            .ForMember(x => x.SortColumn, o => o.MapFrom(s => s.SortColumn ?? string.Empty));
    }
}
