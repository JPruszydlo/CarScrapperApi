namespace CarScrapper.Scrappers
{
    public class PeugeotScrapper : OpelScrapper
    {
        public PeugeotScrapper(string url) : base(url)
        {
            brandName = "Peugeot";
        }
    }
}
