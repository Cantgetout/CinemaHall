using CinemaHub.Data;
using CinemaHub.Data.Models;
using CinemaHub.Data.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CinemaHub.Services
{
    public class ActorService : IActorService
    {
        private readonly ApplicationDbContext _context;

        public ActorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddActorAsync(Actor actor)
        {
            await _context.Actors.AddAsync(actor);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ActorViewModel>> GetAllActorsViewModelAsync()
        {
            return await _context.Actors
                .Select(a => new ActorViewModel
                {
                    Id = a.Id,
                    Name = a.Name
                })
                .OrderBy(a => a.Name)
                .ToListAsync();
        }
        public async Task<List<Actor>> GetAllActorsAsync()
        {
            return await _context.Actors.OrderBy(a => a.Name).ToListAsync();
        }

        public async Task<Actor?> GetActorByIdAsync(int id)
        {
            return await _context.Actors.FindAsync(id);
        }

        public async Task<ActorDetailsViewModel?> GetActorDetailsAsync(int id)
        {
            
            return await _context.Actors
                .Include(a => a.MoviesActors)
                    .ThenInclude(ma => ma.Movie)
                .Where(a => a.Id == id)
                .Select(a => new ActorDetailsViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    Bio = a.Bio,
                    Movies = a.MoviesActors.Select(ma => new MoviePairViewModel
                    {
                        Id = ma.Movie.Id,
                        Title = ma.Movie.Title
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task UpdateActorAsync(Actor actor)
        {
            Actor existingActor = await _context.Actors.FindAsync(actor.Id);

            if (existingActor != null)
            {
                existingActor.Name = actor.Name;
                existingActor.Bio = actor.Bio;

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteActorAsync(int id)
        {
            Actor actor = await _context.Actors.FindAsync(id);
            if (actor != null)
            {
                // EF Core avtomatichno shte iztrie zapisite v mejdinnata tablica MovieActor, t.e. nqma nujda da gi triem ruchno
                _context.Actors.Remove(actor);
                await _context.SaveChangesAsync();
            }
        }
    }
}