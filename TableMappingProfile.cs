using AutoMapper;
using CarScrapper.Entities;
using CarScrapper.Models;

namespace gk_system_api
{
    public class TableMappingProfile : Profile
    {
        public TableMappingProfile()
        {
            CreateMap<CarScrapped, CarScrappedDTO>();
            CreateMap<CarScrappedDTO, CarScrapped>()
                .ForMember(x => x.LastUpdated, opt => opt.MapFrom(src => DateTime.Now));
        }
    }
}
