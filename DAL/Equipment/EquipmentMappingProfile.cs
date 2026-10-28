using AutoMapper;
using Contracts.Equipment;
using BL.Entities;

namespace DAL.Equipment;

/// <summary>
/// Маппинг документа оборудования в запись релейшена и записи релейшена в строку списка
/// </summary>
public class EquipmentMappingProfile : Profile
{
    public EquipmentMappingProfile()
    {
        CreateMap<EquipmentModel, EquipmentEntity>()
            .ForMember(x => x.IsDeleted, o => o.Ignore())
            .ForMember(x => x.UpdateUtcDateTime, o => o.MapFrom(s => s.Timestamp));

        CreateMap<EquipmentEntity, EquipmentRecordItem>()
            .ForMember(x => x.UpdateDate, o => o.MapFrom(s => s.UpdateUtcDateTime));
    }
}
