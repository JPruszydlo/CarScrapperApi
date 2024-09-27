using CarScrapper.Models;
using static CarScrapper.Services.ScrapperService;

namespace CarScrapper.Services.Interfaces
{
    public interface IScrapperService
    {
        bool Scrap(int? id);

        List<CarScrappedDTO> SyncScrapped();

        List<CarScrappedDTO> GetCarsScrapped();
    }
}
