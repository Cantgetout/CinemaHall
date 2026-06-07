namespace CinemaHub.Data.Models.ViewModels
{
    public class MovieDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Year { get; set; }
        public string ImageUrl { get; set; }
        public string Genre { get; set; } = null!;
        public List<ActorPairViewModel> Actors { get; set; } = new List<ActorPairViewModel>();
    }

    // Pomoshten klas samo za tozi klas
    public class ActorPairViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
