namespace CinemaHub.Data.Models.ViewModels
{
    public class ActorDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Bio { get; set; } = null!;
        public List<MoviePairViewModel> Movies { get; set; } = new List<MoviePairViewModel>();
    }

    public class MoviePairViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
    }
}
