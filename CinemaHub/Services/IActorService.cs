using CinemaHub.Data.Models;
using CinemaHub.Data.Models.ViewModels;

namespace CinemaHub.Services
{
    public interface IActorService
    {
        Task<Actor?> GetActorByIdAsync(int id);
        Task AddActorAsync(Actor actor);
        Task UpdateActorAsync(Actor actor);
        Task DeleteActorAsync(int id);
        Task<List<ActorViewModel>> GetAllActorsViewModelAsync();
        Task<List<Actor>> GetAllActorsAsync();

        Task<ActorDetailsViewModel?> GetActorDetailsAsync(int id);
    }
}
