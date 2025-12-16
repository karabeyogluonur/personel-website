using PW.Domain.Entities;

namespace PW.Application.Interfaces.Content
{
    public interface ITechnologyService
    {
        Task<Technology> GetTechnologyByIdAsync(int technologyId);
        Task InsertTechnologyAsync(Technology technology);
        Task<IList<Technology>> GetAllTechnologiesAsync();
        Task UpdateTechnologyAsync(Technology technology);
        Task DeleteTechnologyAsync(Technology technology);
    }
}
