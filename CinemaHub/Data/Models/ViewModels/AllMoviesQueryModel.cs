using System.ComponentModel.DataAnnotations;

namespace CinemaHub.Data.Models.ViewModels
{
    public class AllMoviesQueryModel
    {
        public int MoviesPerPage { get; set; } //kolko filma na stranica (pagination)

        public string? SearchTerm { get; set; } //kakvo se tursi

        [Display(Name = "Search by Genre")]
        public int? GenreId { get; set; } // filtur po janr

        public int CurrentPage { get; set; } = 1; 

        public int TotalMoviesCount { get; set; } // za smqtane na stranicite (obsht broi namereni filnmi)

        public IEnumerable<GenreViewModel> Genres { get; set; } = new List<GenreViewModel>(); //za padashtoto menu

        public List<MovieViewModel> Movies { get; set; } = new List<MovieViewModel>(); // result
    }
}
