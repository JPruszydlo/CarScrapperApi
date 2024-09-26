using CarScrapper.Models;
using static CarScrapper.Services.ScrapperService;

namespace CarScrapper.Services.Interfaces
{
    public interface IScrapperService
    {
        bool Scrap();
        bool Scrap(CarMake carMake);

        List<CarScrappedDTO> SyncScrapped();

        List<CarScrappedDTO> GetCarsScrapped();
    }
}
