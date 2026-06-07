using CinemaHub.Data.Models.ViewModels;

namespace CinemaHub.Services
{
    public interface IScreeningService
    {
        Task<List<ScreeningViewModel>> GetAllScreeningsAsync();  //all
        Task<ScreeningFormModel> GetScreeningFormModelAsync(); // za zarejdane na prazna form s dropdown
        Task AddScreeningAsync(ScreeningFormModel model);   //add
        Task<ScreeningFormModel?> GetScreeningForEditAsync(int id); //edit
        Task UpdateScreeningAsync(ScreeningFormModel model);    //update
        Task DeleteScreeningAsync(int id);  //delete
    }
}
