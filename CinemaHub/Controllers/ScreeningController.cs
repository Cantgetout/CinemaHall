using CinemaHub.Data.Models;
using CinemaHub.Data.Models.ViewModels;
using CinemaHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaHub.Controllers
{
    public class ScreeningsController : Controller
    {
        private readonly IScreeningService _screeningService;

        public ScreeningsController(IScreeningService screeningService)
        {
            _screeningService = screeningService;
        }

        // ALL
        [HttpGet]
        public async Task<IActionResult> All()
        {
            List<ScreeningViewModel> screenings = await _screeningService.GetAllScreeningsAsync();
            return View(screenings);
        }

        // ADD
        [Authorize(Roles = "Administrator, Moderator")]
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            ScreeningFormModel model = await _screeningService.GetScreeningFormModelAsync();
            return View(model);
        }

        [Authorize(Roles = "Administrator, Moderator")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Add(ScreeningFormModel model)
        {
            if (!ModelState.IsValid)
            {
                // Invalid
                ScreeningFormModel dropdowns = await _screeningService.GetScreeningFormModelAsync();
                model.Movies = dropdowns.Movies;
                model.Halls = dropdowns.Halls;
                return View(model);
            }

            await _screeningService.AddScreeningAsync(model);
            TempData["SuccessMessage"] = $"Successfully added screening!";
            return RedirectToAction(nameof(All));
        }

        // EDIT
        [Authorize(Roles = "Administrator, Moderator")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ScreeningFormModel model = await _screeningService.GetScreeningForEditAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [Authorize(Roles = "Administrator, Moderator")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Edit(ScreeningFormModel model)
        {
            if (!ModelState.IsValid)
            {
                //Invalid
                ScreeningFormModel dropdowns = await _screeningService.GetScreeningFormModelAsync();
                model.Movies = dropdowns.Movies;
                model.Halls = dropdowns.Halls;
                return View(model);
            }

            await _screeningService.UpdateScreeningAsync(model);

            TempData["SuccessMessage"] = $"Successfully edited screening!";

            return RedirectToAction(nameof(All));
        }

        // DELETE
        [Authorize(Roles = "Administrator, Moderator")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _screeningService.DeleteScreeningAsync(id);

            TempData["SuccessMessage"] = $"Successfully deleted!";

            return RedirectToAction(nameof(All));
        }
    }
}
