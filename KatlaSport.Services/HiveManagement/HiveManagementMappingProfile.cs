using System;
using AutoMapper;
using DataAccessHive = KatlaSport.DataAccess.ProductStoreHive.StoreHive;
using DataAccessHiveSection = KatlaSport.DataAccess.ProductStoreHive.StoreHiveSection;
using DbHive = KatlaSport.DataAccess.ProductStoreHive.StoreHive;

namespace KatlaSport.Services.HiveManagement
{
    public sealed class HiveManagementMappingProfile : Profile
    {
        public HiveManagementMappingProfile()
        {
            CreateMap<DataAccessHive, HiveListItem>();
            CreateMap<DataAccessHive, Hive>();
            CreateMap<DataAccessHiveSection, HiveSectionListItem>();
            CreateMap<DataAccessHiveSection, HiveSection>();
            CreateMap<UpdateHiveRequest, DataAccessHive>()
                .ForMember(r => r.LastUpdated, opt => opt.MapFrom(p => DateTime.UtcNow));
            CreateMap<UpdateHiveRequest, DbHive>();
        }
    }
}
