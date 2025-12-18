using PW.Application.Models;
using PW.Domain.Entities;

namespace PW.Application.Interfaces.Content
{
    public interface ITechnologyService
    {
        Task<Technology> GetTechnologyByIdAsync(int technologyId);
        Task<IList<Technology>> GetAllTechnologiesAsync();
        Task<OperationResult> InsertTechnologyAsync(Technology technology);
        Task<OperationResult> UpdateTechnologyAsync(Technology technology);
        Task<OperationResult> DeleteTechnologyAsync(Technology technology);
    }
}
