namespace Concessionaire.WebAPI.Dto
{
    public class CarDto
    {
        public CarDto()
        {
        }
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string ImagePath { get; set; }
        public string TechnicalDataSheetPath { get; set; }

        //base64
        public string ImageBase64 { get; set; }
    }
}
