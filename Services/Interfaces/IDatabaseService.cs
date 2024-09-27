using CarScrapper.Entities;
using CarScrapper.Models;

namespace CarScrapper.Services.Interfaces
{
    public interface IDatabaseService
    {
        List<CarScrappedDTO>? GetCarsScrapped();

        IQueryable<string>? GetBrands();

        bool UpdateScrapped(List<CarScrappedDTO> dto, string make);
    }
}
