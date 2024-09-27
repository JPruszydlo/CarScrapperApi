using CarScrapper.Models;
using CarScrapper.Scrappers;
using CarScrapper.Services.Interfaces;

namespace CarScrapper.Services
{
    public class ScrapperService: IScrapperService
    {
        public enum CarMake
        {
            All,
            AlfaRomeo,
            Audi,
            BMW,
            Citroen,
            Cupra,
            Fiat,
            Hyundai,
            Jeep,
            KIA,
            Mazda,
            Mercedes,
            Opel,
            Peugeot,
            Porsche,
            Renault,
            SEAT,
            Skoda,
            Toyota,
            Volkswagen,
        }
        private Dictionary<CarMake, Scrapper> Scrappers = new Dictionary<CarMake, Scrapper>()
        {
            {CarMake.Audi,  new AudiScrapper("https://www.audi.pl/pl/web/pl/modele.html") },
            {CarMake.BMW,  new BmwScrapper("https://www.bmw.pl/pl/all-models.html") },
            {CarMake.AlfaRomeo,  new AlfaRomeoScrapper("https://mwauto.com.pl/alfaromeo/samochody-osobowe/") },
            {CarMake.Citroen,  new CitroenScrapper("https://mwauto.com.pl/citroen/samochody-osobowe/") },
            {CarMake.Cupra,  new CupraScrapper("https://www.cupraofficial.pl/") },
            {CarMake.Fiat,  new FiatScrapper("https://mwauto.com.pl/fiat/samochody-osobowe/") },
            {CarMake.Hyundai,  new HyundaiScrapper("https://www.hyundai.koreamotors.pl/p/modele-hyundai") },
            {CarMake.Jeep,  new JeepScrapper("https://mwauto.com.pl/jeep/samochody-osobowe/") },
            {CarMake.KIA,  new KiaScrapper("https://www.kia.com/pl/modele/kia-range/") },
            {CarMake.Mazda,  new MazdaScrapper("https://www.mazda.pl/skonfiguruj/") },
            {CarMake.Mercedes,  new MercedesScrapper("https://zasadaauto.pl/samochody-osobowe/Mercedes-Benz?status=new&sort=leastExpensive") },
            {CarMake.Opel,  new OpelScrapper("https://mwauto.com.pl/opel/samochody-osobowe/") },
            {CarMake.Peugeot,  new PeugeotScrapper("https://mwauto.com.pl/peugeot/samochody-osobowe/") },
            {CarMake.Porsche,  new PorscheScrapper("https://configurator.porsche.com/pl-PL/model-start") },
            {CarMake.Renault,  new RenaultScrapper("https://www.renault.pl/samochody-osobowe.html") },
            {CarMake.SEAT,  new SeatScrapper("https://www.seat.pl/") },
            {CarMake.Skoda,  new SkodaScrapper("https://www.skoda-auto.pl/modele/poznaj-modele-skoda") },
            {CarMake.Toyota,  new ToyotaScrapper("https://www.toyota.pl/") },
            {CarMake.Volkswagen,  new VolkswagenScrapper("https://www.volkswagen.pl/pl/modele.html") },
        };

        private readonly IDatabaseService _db;
        private readonly ILogger _log;

        public ScrapperService(IDatabaseService db, ILogger<ScrapperService> log)
        {
            _db = db;
            _log = log;
        }

        public bool Scrap()
        {
            var result = new List<CarScrappedDTO>();
            foreach(var scrapper in Scrappers.Values)
            {
                var scrapped = scrapper.GetParsedContent();
                result.AddRange(scrapped);
            }

            return UpdateScrapped(result);
        }

        public bool Scrap(int? id)
        {
            if (id == null)
                return ScrapAll();

            var carMake = (CarMake)id;
            return ScrapSingle(carMake);
        }

        public List<CarScrappedDTO> GetCarsScrapped()
            => _db.GetCarsScrapped() ?? new List<CarScrappedDTO>();

        public List<CarScrappedDTO> SyncScrapped()
        {
            try
            {
                var brands = _db.GetBrands();
                var scrapers = Enum.GetNames(typeof(CarMake)).Where(x => x != "All");
                var result = scrapers.Except(brands).ToList();

                foreach (var item in result)
                {
                    var carMake = (CarMake)Enum.Parse(typeof(CarMake), item);
                    var scrapper = Scrappers[carMake];
                    if (!UpdateScrapped(scrapper.GetParsedContent()))
                    {
                        _log.LogError($"Error during sync {item}");
                        continue;
                    }
                }

                return _db.GetCarsScrapped() ?? new List<CarScrappedDTO>();
            }
            catch (Exception ex)
            {
                _log.LogError(ex.ToString());
                return new List<CarScrappedDTO>();
            }
        }
        private bool ScrapAll()
        {
            try
            {
                var result = new List<CarScrappedDTO>();
                foreach (var scrapper in Scrappers.Values)
                {
                    var scrapped = scrapper.GetParsedContent();
                    result.AddRange(scrapped);
                }

                return UpdateScrapped(result);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.ToString());
                return false;
            }
        }
        private bool ScrapSingle(CarMake carMake)
        {
            try
            {
                return UpdateScrapped(Scrappers[carMake].GetParsedContent());
            }
            catch(Exception ex)
            {
                _log.LogError(ex.ToString());
                return false;
            }
        }

        private bool UpdateScrapped(List<CarScrappedDTO> dto)
        {
            try
            {
                var makes = dto.GroupBy(x => x.Make).Select(x => x.Key).ToList();
                foreach(var make in makes)
                {
                    _db.UpdateScrapped(dto, make);
                }
                return true;
            }
            catch(Exception ex)
            {
                _log.LogError("ScrapperService.UpdateScrapped => " + ex.ToString());
                return false;
            }
        }
    }
}
