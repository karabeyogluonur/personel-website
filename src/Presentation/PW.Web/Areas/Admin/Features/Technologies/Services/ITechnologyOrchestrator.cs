using PW.Application.Models;
using PW.Web.Areas.Admin.Features.Technologies.ViewModels;
namespace PW.Web.Areas.Admin.Features.Technologies.Services;

public interface ITechnologyOrchestrator
{
   Task<OperationResult<TechnologyListViewModel>> PrepareTechnologyListViewModelAsync();
   Task<OperationResult<TechnologyCreateViewModel>> PrepareCreateViewModelAsync(TechnologyCreateViewModel? technologyCreateViewModel = null);
   Task<OperationResult> CreateTechnologyAsync(TechnologyCreateViewModel technologyCreateViewModel);
   Task<OperationResult<TechnologyEditViewModel>> PrepareEditViewModelAsync(int technologyId, TechnologyEditViewModel? technologyEditViewModel = null);
   Task<OperationResult> UpdateTechnologyAsync(TechnologyEditViewModel technologyEditViewModel);
   Task<OperationResult> DeleteTechnologyAsync(int technologyId);
}
