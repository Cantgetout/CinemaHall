using CinemaHub.Data.Models.ViewModels;
using CinemaHub.Data.Models;
using CinemaHub.Data;
using Microsoft.EntityFrameworkCore;

namespace CinemaHub.Services
{
    public class ScreeningService : IScreeningService
    {
        private readonly ApplicationDbContext _context;

        public ScreeningService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ScreeningViewModel>> GetAllScreeningsAsync()
        {
            return await _context.Screenings
                .Include(s => s.Movie)
                .Include(s => s.CinemaHall)
                .OrderBy(s => s.StartDateTime)
                .Select(s => new ScreeningViewModel
                {
                    Id = s.Id,
                    MovieTitle = s.Movie.Title,
                    HallName = s.CinemaHall.Name,
                    StartDateTime = s.StartDateTime.ToString("dd.MM.yyyy HH:mm") // Format za data
                })
                .ToListAsync();
        }

        // tozi metod podgotvq dannite za padashti menuta
        public async Task<ScreeningFormModel> GetScreeningFormModelAsync()
        {
            return new ScreeningFormModel
            {
                StartDateTime = DateTime.Now.AddHours(1), // chas v budeshteto by default
                Movies = await _context.Movies.Select(m => new MoviePairViewModel { Id = m.Id, Title = m.Title }).ToListAsync(),
                Halls = await _context.CinemaHalls.Select(h => new CinemaHallViewModel { Id = h.Id, Name = h.Name }).ToListAsync()
            };
        }

        public async Task AddScreeningAsync(ScreeningFormModel model)
        {
            Screening screening = new Screening
            {
                MovieId = model.MovieId,
                CinemaHallId = model.CinemaHallId,
                StartDateTime = model.StartDateTime
            };
            await _context.Screenings.AddAsync(screening);
            await _context.SaveChangesAsync();
        }

        public async Task<ScreeningFormModel?> GetScreeningForEditAsync(int id)
        {
            Screening screening = await _context.Screenings.FindAsync(id);
            if (screening == null) return null;

            ScreeningFormModel model = await GetScreeningFormModelAsync(); 
            model.Id = screening.Id;
            model.MovieId = screening.MovieId;
            model.CinemaHallId = screening.CinemaHallId;
            model.StartDateTime = screening.StartDateTime;

            return model;
        }

        public async Task UpdateScreeningAsync(ScreeningFormModel model)
        {
            Screening screening = await _context.Screenings.FindAsync(model.Id);
            if (screening != null)
            {
                screening.MovieId = model.MovieId;
                screening.CinemaHallId = model.CinemaHallId;
                screening.StartDateTime = model.StartDateTime;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteScreeningAsync(int id)
        {
            Screening screening = await _context.Screenings.FindAsync(id);
            if (screening != null)
            {
                _context.Screenings.Remove(screening);
                await _context.SaveChangesAsync();
            }
        }
    }
}
