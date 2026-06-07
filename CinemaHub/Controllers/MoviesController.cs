using CinemaHub.Data.Models.ViewModels;
using CinemaHub.Models.ViewModels;
using CinemaHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CinemaHub.Controllers
{
    public class MoviesController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly IActorService _actorService;
        private readonly IConfiguration _config;

        // Dependency Injection - tuk poluchavame service-a. Kogato se poiska IMovieService se podava MovieService ot application-a avtomatichno
        public MoviesController(IMovieService movieService, IActorService actorService, IConfiguration config)
        {
            _movieService = movieService;
            _actorService = actorService;
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> All([FromQuery] AllMoviesQueryModel query)
        {
            // Взимаме стойността от appsettings.json (ако я няма, ползваме 3 по подразбиране)
            int moviesPerPage = _config.GetValue<int>("Pagination:MoviesPerPage", 3);
            query.MoviesPerPage = moviesPerPage;

            var result = await _movieService.GetAllMoviesAsync(
                query.SearchTerm,
                query.GenreId,
                query.CurrentPage,
                moviesPerPage
            );

            // Ot bazata chrez filtriranoto query vrushtame resultatite kum query, koito otiva kum view-to  
            query.Movies = result.Movies;
            query.TotalMoviesCount = result.TotalMoviesCount;
            query.Genres = await _movieService.GetGenresAsync();    //kakto i genres
            return View(query);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var model = await _movieService.GetMovieDetailsAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [Authorize(Roles = "Administrator, Moderator")]
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            //suzdavane na model, koito shte bude podaden na view-to
            var model = new AddMovieViewModel
            {
                Genres = await _movieService.GetGenresAsync(),
                AllActors = await _actorService.GetAllActorsViewModelAsync()
            };

            return View("Add", model);
        }

        [Authorize(Roles = "Administrator, Moderator")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Add(AddMovieViewModel model)
        {
            if (!ModelState.IsValid)    //pravi proverka dali vsichki poleta sa vuvedeni pravilno spored pravilata, zadadeni vuv View modela
            {
                //ako ima netochnost
                model.Genres = await _movieService.GetGenresAsync();
                model.AllActors = await _actorService.GetAllActorsViewModelAsync();
                return View(model);
            }

            try
            {
                await _movieService.AddMovieAsync(model);   //dobavqne kym db

                TempData["SuccessMessage"] = $"Successfully added '{model.Title}' to the catalog!";     //toast

                return RedirectToAction("Index", "Home"); // Sled uspeshno dobavqne shte redirectna kum Home page
            }
            catch (Exception)
            {
                //ako neshto e grumnalo
                ModelState.AddModelError("", "Something went wrong adding the movie.");

                //aide pak kum view-to
                model.Genres = await _movieService.GetGenresAsync();
                model.AllActors = await _actorService.GetAllActorsViewModelAsync();
                return View(model);
            }
        }



        [Authorize(Roles = "Administrator, Moderator")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _movieService.GetMovieForEditAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            //zarejdame tezi poleta za modela tuk, a ne v service-a, zaradi separation of concerns
            model.Genres = await _movieService.GetGenresAsync();
            model.AllActors = await _actorService.GetAllActorsViewModelAsync();

            return View("Edit", model);
        }

        [Authorize(Roles = "Administrator, Moderator")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Edit(EditMovieViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //ako ima netochnost
                model.Genres = await _movieService.GetGenresAsync();
                model.AllActors = await _actorService.GetAllActorsViewModelAsync();
                return View(model);
            }
            try
            {
                await _movieService.UpdateMovieAsync(model);

                TempData["SuccessMessage"] = $"Successfully edited '{model.Title}'!";

                return RedirectToAction("All");
            }
            catch (Exception)
            {
                //ako neshto e grumnalo
                ModelState.AddModelError("", "Something went wrong editing the movie.");

                //aide pak kum view-to
                model.Genres = await _movieService.GetGenresAsync();
                model.AllActors = await _actorService.GetAllActorsViewModelAsync();
                return View(model);

            }
        }

        [Authorize(Roles = "Administrator, Moderator")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _movieService.DeleteMovieAsync(id);

            TempData["SuccessMessage"] = $"Successfully deleted!";

            return RedirectToAction("All");
        }
    }
}





/*
1. Как разбира кое View да отвори?
Когато напишеш просто return View(model); без да уточняваш име на файл, ASP.NET Core прави следното разследване:

Вижда името на контролера: Твоят клас се казва MoviesController. Framework-ът маха суфикса "Controller" и остава с "Movies". Това е името на папката, която ще търси.

Вижда името на метода (Action): Методът ти се казва Add. Това е името на файла, който ще търси.

Сглобява пътя: На базата на горните две, той автоматично търси файл на следния адрес: Views/Movies/Add.cshtml
*/