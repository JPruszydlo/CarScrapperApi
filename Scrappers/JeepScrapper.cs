namespace CarScrapper.Scrappers
{
    public class JeepScrapper : OpelScrapper
    {
        public JeepScrapper(string url) : base(url)
        {
            brandName = "Jeep";
        }
    }
}
