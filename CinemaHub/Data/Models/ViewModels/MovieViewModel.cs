namespace CinemaHub.Data.Models.ViewModels
{
    public class MovieViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Year { get; set; }
        public string ImageUrl { get; set; }
        public string Genre { get; set; } = null!; // za imeto na janra
        public List<string> Actors { get; set; } = new List<string>();  //spisuk s imena na aktiorite

    }
}
