using CinemaHub.Data.Models.ViewModels;
using CinemaHub.Models.ViewModels;

namespace CinemaHub.Services
{
    public interface IMovieService
    {
        //za vzimane na vsichki genres
        Task<List<GenreViewModel>> GetGenresAsync();

        // za dobavqne na film v db
        Task AddMovieAsync(AddMovieViewModel model);

        //za vzimane na vsichki movies
        Task<AllMoviesQueryModel> GetAllMoviesAsync(
            string? searchTerm = null,
            int? genreId = null,
            int currentPage = 1,
            int moviesPerPage = 10);

        //vzimane na EditMovie model ot bazata sprqmo id
        Task<EditMovieViewModel?> GetMovieForEditAsync(int id);

        //update
        Task UpdateMovieAsync(EditMovieViewModel model);

        //delete 
        Task DeleteMovieAsync(int id);
        
        //Deatils
        Task<MovieDetailsViewModel?> GetMovieDetailsAsync(int id);
    }
}