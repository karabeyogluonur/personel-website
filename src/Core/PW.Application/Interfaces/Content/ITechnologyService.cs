using PW.Application.Models;
using PW.Application.Models.Dtos.Content;

namespace PW.Application.Interfaces.Content;

public interface ITechnologyService
{
   Task<IList<TechnologySummaryDto>> GetAllTechnologiesAsync();
   Task<TechnologyDetailDto?> GetTechnologyByIdAsync(int technologyId);
   Task<OperationResult> CreateTechnologyAsync(TechnologyCreateDto technologyCreateDto);
   Task<OperationResult> UpdateTechnologyAsync(TechnologyUpdateDto technologyUpdateDto);
   Task<OperationResult> DeleteTechnologyAsync(int technologyId);
}
