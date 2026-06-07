using System.ComponentModel.DataAnnotations;

namespace CinemaHub.Data.Models.ViewModels
{
    public class ScreeningFormModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Movie")]
        public int MovieId { get; set; }

        [Required]
        [Display(Name = "Cinema Hall")]
        public int CinemaHallId { get; set; }

        [Required]
        [Display(Name = "Date and Time")]
        public DateTime StartDateTime { get; set; }

        // Za padashti menuta
        public IEnumerable<MoviePairViewModel>? Movies { get; set; } //polzvame sushtestvuvashtiq movie pomoshten klas za pair
        public IEnumerable<CinemaHallViewModel>? Halls { get; set; }
    }

    // Pomoshten klas za zalite
    public class CinemaHallViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
