namespace CinemaHub.Data.Models.ViewModels
{
    public class ScreeningViewModel
    {
        public int Id { get; set; }
        public string MovieTitle { get; set; } = null!;
        public string HallName { get; set; } = null!;
        public string StartDateTime { get; set; } = null!; // Shte bude formatiran kato string
    }
}
