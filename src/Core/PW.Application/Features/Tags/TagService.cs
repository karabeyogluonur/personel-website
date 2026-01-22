using Microsoft.EntityFrameworkCore;
using PW.Application.Common.Enums;
using PW.Application.Common.Extensions;
using PW.Application.Features.Tags.Dtos;
using PW.Application.Interfaces.Repositories;
using PW.Application.Utilities.Results;
using PW.Domain.Entities;

namespace PW.Application.Features.Tags;

public class TagService : ITagService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Tag> _tagRepository;

    public TagService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _tagRepository = _unitOfWork.GetRepository<Tag>();
    }

    public async Task<IList<TagSummaryDto>> GetAllTagsAsync()
    {
        IList<Tag> tags = await _tagRepository.GetAllAsync(
            predicate: tag => !tag.IsDeleted,
            orderBy: query => query.OrderByDescending(tag => tag.CreatedAt)
        );

        IList<TagSummaryDto> tagSummaryDtos = tags.Select(tag => new TagSummaryDto
        {
            Id = tag.Id,
            CreatedAt = tag.CreatedAt,
            Name = tag.Name,
            UpdatedAt = tag.UpdatedAt,
        }).ToList();

        return tagSummaryDtos;
    }

    public async Task<TagDetailDto?> GetTagByIdAsync(int tagId)
    {
        if (tagId <= 0) return null;

        Tag tag = await _tagRepository.GetFirstOrDefaultAsync(
            predicate: tag => tag.Id == tagId,
            include: source => source.Include(tag => tag.Translations)
        );

        if (tag is null) return null;

        return new TagDetailDto
        {
            Id = tag.Id,
            IsActive = tag.IsActive,
            Name = tag.Name,
            Description = tag.Description,
            ColorHex = tag.ColorHex,
            CreatedAt = tag.CreatedAt,
            DeletedAt = tag.DeletedAt,
            IsDeleted = tag.IsDeleted,
            UpdatedAt = tag.UpdatedAt,
            Translations = tag.Translations.Select(translation => new TagTranslationDto
            {
                LanguageId = translation.LanguageId,
                Name = translation.Name,
                Description = translation.Description,
            }).ToList()
        };
    }

    public async Task<OperationResult> CreateTagAsync(TagCreateDto tagCreateDto)
    {
        if (tagCreateDto == null)
            throw new ArgumentNullException(nameof(tagCreateDto));

        Tag tag = new Tag
        {
            Name = tagCreateDto.Name,
            Description = tagCreateDto.Description,
            CreatedAt = DateTime.UtcNow,
            ColorHex = tagCreateDto.ColorHex,
            IsDeleted = false,
            Translations = new List<TagTranslation>()
        };

        tag.Translations.SyncTranslations(
            translationDtos: tagCreateDto.Translations,
            isEmptyPredicate: (TagTranslationDto translationDto) =>
                string.IsNullOrWhiteSpace(translationDto.Name) &&
                string.IsNullOrWhiteSpace(translationDto.Description),
            mapAction: (TagTranslation translation, TagTranslationDto translationDto) =>
            {
                translation.Name = string.IsNullOrWhiteSpace(translationDto.Name) ? null : translationDto.Name;
                translation.Description = string.IsNullOrWhiteSpace(translationDto.Description) ? null : translationDto.Description;
            }
        );

        await _tagRepository.InsertAsync(tag);
        await _unitOfWork.CommitAsync();
        return OperationResult.Success();
    }

    public async Task<OperationResult> UpdateTagAsync(TagUpdateDto tagUpdateDto)
    {
        if (tagUpdateDto == null)
            throw new ArgumentNullException(nameof(tagUpdateDto));

        Tag tag = await _tagRepository.GetFirstOrDefaultAsync(
            predicate: tag => tag.Id == tagUpdateDto.Id,
            include: source => source.Include(tag => tag.Translations),
            disableTracking: false
        );

        if (tag is null)
            return OperationResult.Failure("Tag not found.", OperationErrorType.NotFound);

        tag.Name = tagUpdateDto.Name;
        tag.ColorHex = tagUpdateDto.ColorHex;
        tag.Description = tagUpdateDto.Description;
        tag.Translations.SyncTranslations(
            translationDtos: tagUpdateDto.Translations,

            isEmptyPredicate: (TagTranslationDto translationDto) =>
                string.IsNullOrWhiteSpace(translationDto.Name) &&
                string.IsNullOrWhiteSpace(translationDto.Description),

            mapAction: (TagTranslation translation, TagTranslationDto translationDto) =>
            {
                translation.Name = string.IsNullOrWhiteSpace(translationDto.Name) ? null : translationDto.Name;
                translation.Description = string.IsNullOrWhiteSpace(translationDto.Description) ? null : translationDto.Description;
            }
        );

        await _unitOfWork.CommitAsync();

        return OperationResult.Success();
    }

    public async Task<OperationResult> DeleteTagAsync(int tagId)
    {
        Tag tag = await _tagRepository.GetFirstOrDefaultAsync(
            predicate: tag => tag.Id == tagId,
            include: source => source.Include(tag => tag.Translations)
        );

        if (tag == null)
            return OperationResult.Failure("Tag not found.", OperationErrorType.NotFound);

        _tagRepository.Delete(tag);
        await _unitOfWork.CommitAsync();

        return OperationResult.Success();
    }
}
