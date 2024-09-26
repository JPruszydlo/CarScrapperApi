namespace CarScrapper.Scrappers
{
    public class FiatScrapper : OpelScrapper
    {
        public FiatScrapper(string url) : base(url)
        {
            brandName = "Fiat";
        }
    }
}
