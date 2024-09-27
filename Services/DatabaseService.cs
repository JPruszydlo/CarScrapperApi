using AutoMapper;
using CarScrapper.Entities;
using CarScrapper.Models;
using CarScrapper.Services.Interfaces;

namespace CarScrapper.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly IMapper _mapper;
        private readonly ILogger _log;
        private readonly ScrapperDbContext _db;
        public DatabaseService(ScrapperDbContext db, ILogger<DatabaseService> log, IMapper mapper)
        {
            _db = db;
            _log = log;
            _mapper = mapper;
        }
        public bool UpdateScrapped(List<CarScrappedDTO> dto, string make)
        {
            try
            {
                var cars = _db.Cars.Where(x => x.Make == make);
                if (cars.Any())
                    _db.RemoveRange(cars);
                var mapped = _mapper.Map<List<CarScrapped>>(dto);
                _db.Cars.AddRange(mapped);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.ToString());
                return false;
            }
        }

        public IQueryable<string>? GetBrands()
        {
            try
            {
                return _db.Cars.Select(x => x.Make).Distinct();
            }
            catch(Exception ex)
            {
                _log.LogError("DatabaseService.GetBrands() => " + ex.ToString());
                return null;
            }
        }

        public List<CarScrappedDTO>? GetCarsScrapped()
        {
            try
            {
                _log.LogWarning("ASDASDASDSASASDAD");
                return _mapper.Map<List<CarScrappedDTO>>(_db.Cars);
            }
            catch(Exception ex)
            {
                _log.LogError("DatabaseService.GetCarsScrapped() => " + ex.ToString());
                return null;
            }
        }
    }
}
