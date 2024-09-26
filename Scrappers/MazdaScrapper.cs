using CarScrapper.Models;
using Newtonsoft.Json;

namespace CarScrapper.Scrappers
{
    public class MazdaScrapper : Scrapper
    {
        public MazdaScrapper(string url) : base(url)
        { }

        public override List<CarScrappedDTO> GetParsedContent()
        {
            var doc = base.GetContent();
            var tmp = doc.DocumentNode.Descendants("script")
                .Where(n => n.InnerText.Contains("vehicleName"))
                .First().InnerText;

            var json = tmp.Substring(tmp.IndexOf("JSON.parse('") + "JSON.parse('".Length);
            json = json.Remove(json.Length - 3);

            var deserialized = JsonConvert.DeserializeObject<Root>(json);
            return GetCars(deserialized.props.cars);
        }

        private List<CarScrappedDTO> GetCars(List<Car> cars)
            => cars.Select(car => new CarScrappedDTO
            {
                Make = "Mazda",
                Model = car.vehicleName.Split(new char[0]).LastOrDefault(),
                Link = "https://www.mazda.pl" + car.url,
                Image = car.image.url,
                Price = car.price.priceValue
            }).ToList();
    }
    public class AssetTransformationInfo
    {
        public int originalWidth { get; set; }
        public int originalHeight { get; set; }
        public string assetUrl { get; set; }
        public FocalPoint focalPoint { get; set; }
    }

    public class Car
    {
        public string id { get; set; }
        public string carId { get; set; }
        public Image image { get; set; }
        public MobileImage mobileImage { get; set; }
        public string promotionalText { get; set; }
        public string promotionalTextColor { get; set; }
        public string vehicleName { get; set; }
        public bool showPricePrefix { get; set; }
        public Price price { get; set; }
        public string url { get; set; }
        public string urlTarget { get; set; }
        public string bodyName { get; set; }
    }

    public class ContinueConfiguration
    {
        public string findConfigurationValidationRegexp { get; set; }
        public bool useFindConfiguration { get; set; }
        public string configuratorBaseUrl { get; set; }
        public Translations translations { get; set; }
        public bool showModelNameOnly { get; set; }
    }

    public class FocalPoint
    {
        public double x { get; set; }
        public double y { get; set; }
    }

    public class Image
    {
        public AssetTransformationInfo assetTransformationInfo { get; set; }
        public string url { get; set; }
    }

    public class MobileImage
    {
        public AssetTransformationInfo assetTransformationInfo { get; set; }
        public string url { get; set; }
    }

    public class Price
    {
        public double priceValue { get; set; }
        public double rawPrice { get; set; }
        public string currency { get; set; }
        public string formattedPrice { get; set; }
        public string superscript { get; set; }
    }

    public class Props
    {
        public string id { get; set; }
        public string headerText { get; set; }
        public ContinueConfiguration continueConfiguration { get; set; }
        public List<Car> cars { get; set; }
        public Translations translations { get; set; }
    }

    public class Root
    {
        public Props props { get; set; }
        public string type { get; set; }
    }

    public class Translations
    {
        public string title { get; set; }
        public string continueTitle { get; set; }
        public string continueCtaText { get; set; }
        public string startNewCtaText { get; set; }
        public string findConfigurationTitle { get; set; }
        public string findConfigurationDescription { get; set; }
        public string findConfigurationInputWatermark { get; set; }
        public string findConfigurationCtaText { get; set; }
        public string findConfigurationValidationError { get; set; }
        public string from { get; set; }
    }
}
