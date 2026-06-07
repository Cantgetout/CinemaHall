using CinemaHub.Data;
using CinemaHub.Data.Models.ViewModels;
using CinemaHub.Data.Models;
using Microsoft.EntityFrameworkCore;
using CinemaHub.Models.ViewModels;

namespace CinemaHub.Services
{
    public class MovieService : IMovieService
    {
        private readonly ApplicationDbContext _context;

        //ctor
        public MovieService(ApplicationDbContext context)
        {
            _context = context;
        }

        //Task<List<...>>. Vrushtash obeshtanie, che shte doide spisukut s elementi sled obrabotkata
        public async Task<List<GenreViewModel>> GetGenresAsync() //pochti vsichko v .NET e async, kogato se govori za vruzka s bazata danni. Ideqta e che dokato se chaka vruzkata s bazata, survurut moje da obsluji drugi potrebiteli
        {
            return await _context.Genres
                .Select(g => new GenreViewModel
                {
                    Id = g.Id,
                    Name = g.Name
                })
                .ToListAsync();
        }

        public async Task AddMovieAsync(AddMovieViewModel model)
        {
            // Prehvurlqme dannite ot viewModel kum movie v bazata danni
            Movie movie = new Movie
            {
                Title = model.Title,
                Description = model.Description,
                Year = model.Year,
                ImageUrl = model.ImageUrl,
                GenreId = model.GenreId
            };

            foreach (int actorId in model.SelectedActorIds)
            {
                MovieActor movieActor = new MovieActor
                {
                    Movie = movie,    // EF Core sam shte se opravi i shte razbere id-to na noviq film
                    ActorId = actorId
                };

                movie.MoviesActors.Add(movieActor);
            }

            await _context.Movies.AddAsync(movie);
            await _context.SaveChangesAsync();
        }

        public async Task<AllMoviesQueryModel> GetAllMoviesAsync(
            string? searchTerm = null,
            int? genreId = null,
            int currentPage = 1,
            int moviesPerPage = 10)
        {
            // 1 suzdavane na query
            var moviesQuery = _context.Movies.AsQueryable();

            // 2. Ako ima janr filtirai po nego
            if (genreId != null)
            {
                moviesQuery = moviesQuery.Where(m => m.GenreId == genreId.Value);
            }

            // 3. Ako user-a e napisal neshto filtrirai zaglvieto da go sudurja
            if (!string.IsNullOrEmpty(searchTerm))
            {
                string wildCard = searchTerm.ToLower();
                moviesQuery = moviesQuery.Where(m => m.Title.ToLower().Contains(wildCard));
            }

            // 4. Sortirane, po nai-novi. Zaduljitelno e za pagination-a
            moviesQuery = moviesQuery.OrderByDescending(m => m.Id); // Най-новите най-отгоре

            // Prebroqvane na broqt rezultati
            var totalMovies = await moviesQuery.CountAsync();

            // 6 Pagination (skip[ i  take)
            List<MovieViewModel> movies = await moviesQuery
                .Skip((currentPage - 1) * moviesPerPage)
                .Take(moviesPerPage)
                .Select(m => new MovieViewModel
                {
                    Id = m.Id,
                    Title = m.Title,
                    Genre = m.Genre.Name,
                    Description = m.Description,
                    ImageUrl = m.ImageUrl,
                    Year = m.Year,
                    Actors = m.MoviesActors.Select(ma => ma.Actor.Name).ToList()
                })
                .ToListAsync();

            // 7. Връщаме пълния модел
            return new AllMoviesQueryModel
            {
                Movies = movies,
                TotalMoviesCount = totalMovies,
                CurrentPage = currentPage,
                SearchTerm = searchTerm, // Vrushtame obratno genre i searchTerm za da ostanat populneni vuv form-a
                GenreId = genreId
            };
        }

        public async Task<EditMovieViewModel?> GetMovieForEditAsync(int id)
        {
            var movie = await _context.Movies       //nqma include(m => m.genre) suotvetno imame samo id na genre, bez da mojem da dostupim poletata mu
                .Include(m => m.MoviesActors)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return null;

            return new EditMovieViewModel
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                Year = movie.Year,
                ImageUrl = movie.ImageUrl,
                GenreId = movie.GenreId,
                SelectedActorIds = movie.MoviesActors.Select(ma => ma.ActorId).ToList()
            };
        }

        public async Task<MovieDetailsViewModel?> GetMovieDetailsAsync(int id)
        {
            return await _context.Movies
                .Include(m => m.Genre)
                .Include(m => m.MoviesActors)
                    .ThenInclude(ma => ma.Actor)
                .Where(m => m.Id == id)
                .Select(m => new MovieDetailsViewModel
                {
                    Id = m.Id,
                    Title = m.Title,
                    Description = m.Description,
                    Year = m.Year,
                    ImageUrl = m.ImageUrl,
                    Genre = m.Genre.Name,
                    Actors = m.MoviesActors.Select(ma => new ActorPairViewModel
                    {
                        Id = ma.Actor.Id,
                        Name = ma.Actor.Name
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task UpdateMovieAsync(EditMovieViewModel model)
        {
            var movie = await _context.Movies
                .Include(m => m.MoviesActors)
                .FirstOrDefaultAsync(m => m.Id == model.Id);

            if (movie == null) return;

            movie.Title = model.Title;
            movie.Description = model.Description;
            movie.Year = model.Year;
            movie.ImageUrl = model.ImageUrl;
            movie.GenreId = model.GenreId;

            // Many to many update logic ---

            //mahame starite aktiori
            movie.MoviesActors.Clear();

            // dobavqme aktiorite ot input-a na user-a
            foreach (var actorId in model.SelectedActorIds)
            {
                movie.MoviesActors.Add(new MovieActor
                {
                    MovieId = movie.Id,
                    ActorId = actorId
                });
            }

                await _context.SaveChangesAsync();
        }

        public async Task DeleteMovieAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null) return;

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
        }
    }
}



/*
 * Вариант 1: С .Include() (Твоят код)

EF Core вижда Include, казва си "Ок, ще дръпна данните".

После вижда Select и си казва: "А, чакай, той всъщност иска само тези 3 полета, а не целия обект. Игнорирам Include-а и правя оптимизирана заявка само за Select-а."

Резултат: Работи, но си написал излишен код.

Вариант 2: Само .Select()

EF Core вижда m.Genre.Name и m.MoviesActors... вътре в селекта.

Казва си: "Трябва ми Genre, значи правя JOIN. Трябват ми актьори, правя JOIN."

Резултат: Същата SQL заявка, по-малко C# код.
 * */