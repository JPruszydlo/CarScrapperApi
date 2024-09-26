namespace CarScrapper.Models
{
    public class CarScrappedDTO
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public string Link { get; set; }
        public string Image { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
