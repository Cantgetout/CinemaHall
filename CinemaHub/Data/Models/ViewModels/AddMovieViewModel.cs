using System.ComponentModel.DataAnnotations;

namespace CinemaHub.Data.Models.ViewModels
{
    public class AddMovieViewModel
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(500, MinimumLength = 10)]
        public string Description { get; set; } = null!;

        [Range(1900, 2030)]
        public int Year { get; set; }

        [Url]
        public string? ImageUrl { get; set; }

        [Display(Name = "Genre")]
        public int GenreId { get; set; }

        //Za padashto menu s janrove
        public List<GenreViewModel>? Genres { get; set; }

        // Shte pazi spisuk s vsichki vuzmojni aktiori
        public List<ActorViewModel>? AllActors { get; set; }

        // Id-tata na aktiorite, selectnati ot user-a
        public List<int> SelectedActorIds { get; set; } = new List<int>();
    }
}