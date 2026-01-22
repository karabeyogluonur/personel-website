using PW.Application.Features.Tags.Dtos;
using PW.Application.Utilities.Results;

namespace PW.Application.Features.Tags;

public interface ITagService
{
    Task<TagDetailDto?> GetTagByIdAsync(int tagId);
    Task<IList<TagSummaryDto>> GetAllTagsAsync();
    Task<OperationResult> CreateTagAsync(TagCreateDto tagCreateDto);
    Task<OperationResult> UpdateTagAsync(TagUpdateDto tagUpdateDto);
    Task<OperationResult> DeleteTagAsync(int tagId);
}
