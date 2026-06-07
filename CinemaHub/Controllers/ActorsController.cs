using CinemaHub.Data.Models; // Или CinemaHub.Models
using CinemaHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaHub.Controllers
{
    [Authorize]
    public class ActorsController : Controller
    {
        private readonly IActorService _actorService;

        public ActorsController(IActorService actorService)
        {
            _actorService = actorService;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            List<Actor> actors = await _actorService.GetAllActorsAsync();
            return View(actors);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var model = await _actorService.GetActorDetailsAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [Authorize(Roles = "Administrator, Moderator")]
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [Authorize(Roles = "Administrator, Moderator")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Add(Actor actor)
        {
            if (!ModelState.IsValid)
            {
                return View(actor);
            }

            await _actorService.AddActorAsync(actor);

            TempData["SuccessMessage"] = $"Successfully added '{actor.Name}' to the catalog!";

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Administrator, Moderator")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var actor = await _actorService.GetActorByIdAsync(id);
            if (actor == null) return NotFound();

            return View(actor);
        }

        [Authorize(Roles = "Administrator, Moderator")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Edit(Actor actor)
        {
            if (!ModelState.IsValid) return View(actor);

            await _actorService.UpdateActorAsync(actor);

            TempData["SuccessMessage"] = $"Successfully edited '{actor.Name}'!";

            return RedirectToAction("Index", "Home"); ;
        }

        // 4. DELETE
        [Authorize(Roles = "Administrator, Moderator")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _actorService.DeleteActorAsync(id);

            TempData["SuccessMessage"] = $"Successfully deleted!";

            return RedirectToAction(nameof(All));
        }

    }
}