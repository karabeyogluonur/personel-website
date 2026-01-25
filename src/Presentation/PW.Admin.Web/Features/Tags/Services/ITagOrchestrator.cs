using PW.Application.Utilities.Results;
using PW.Admin.Web.Features.Tags.ViewModels;

namespace PW.Admin.Web.Features.Tags.Services;

public interface ITagOrchestrator
{
    Task<OperationResult<TagListViewModel>> PrepareTagListViewModelAsync();
    Task<OperationResult<TagCreateViewModel>> PrepareCreateViewModelAsync(TagCreateViewModel? tagCreateViewModel = null);
    Task<OperationResult> CreateTagAsync(TagCreateViewModel tagCreateViewModel);
    Task<OperationResult<TagEditViewModel>> PrepareEditViewModelAsync(int tagId, TagEditViewModel? tagEditViewModel = null);
    Task<OperationResult> UpdateTagAsync(TagEditViewModel tagEditViewModel);
    Task<OperationResult> DeleteTagAsync(int tagId);
}
