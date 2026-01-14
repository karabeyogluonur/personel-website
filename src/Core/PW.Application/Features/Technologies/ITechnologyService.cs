using PW.Application.Features.Technologies.Dtos;
using PW.Application.Utilities.Results;

namespace PW.Application.Features.Technologies;

public interface ITechnologyService
{
   Task<IList<TechnologySummaryDto>> GetAllTechnologiesAsync();
   Task<TechnologyDetailDto?> GetTechnologyByIdAsync(int technologyId);
   Task<OperationResult> CreateTechnologyAsync(TechnologyCreateDto technologyCreateDto);
   Task<OperationResult> UpdateTechnologyAsync(TechnologyUpdateDto technologyUpdateDto);
   Task<OperationResult> DeleteTechnologyAsync(int technologyId);
}
