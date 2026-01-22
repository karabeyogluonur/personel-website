using PW.Application.Utilities.Results;
using PW.Web.Areas.Admin.Features.Tags.ViewModels;

namespace PW.Web.Areas.Admin.Features.Tags.Services;

public interface ITagOrchestrator
{
    Task<OperationResult<TagListViewModel>> PrepareTagListViewModelAsync();
    Task<OperationResult<TagCreateViewModel>> PrepareCreateViewModelAsync(TagCreateViewModel? tagCreateViewModel = null);
    Task<OperationResult> CreateTagAsync(TagCreateViewModel tagCreateViewModel);
    Task<OperationResult<TagEditViewModel>> PrepareEditViewModelAsync(int tagId, TagEditViewModel? tagEditViewModel = null);
    Task<OperationResult> UpdateTagAsync(TagEditViewModel tagEditViewModel);
    Task<OperationResult> DeleteTagAsync(int tagId);
}
