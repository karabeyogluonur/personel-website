using AutoMapper;
using PW.Application.Common.Enums;
using PW.Application.Features.Localization;
using PW.Application.Features.Localization.Dtos;
using PW.Application.Features.Tags;
using PW.Application.Features.Tags.Dtos;
using PW.Application.Utilities.Results;
using PW.Web.Areas.Admin.Features.Common.Models;
using PW.Web.Areas.Admin.Features.Tags.ViewModels;

namespace PW.Web.Areas.Admin.Features.Tags.Services;

public class TagOrchestrator : ITagOrchestrator
{
    private readonly ITagService _tagService;
    private readonly ILanguageService _languageService;
    private readonly IMapper _mapper;

    public TagOrchestrator(ITagService tagService, ILanguageService languageService, IMapper mapper)
    {
        _tagService = tagService;
        _languageService = languageService;
        _mapper = mapper;
    }

    public async Task<OperationResult<TagListViewModel>> PrepareTagListViewModelAsync()
    {
        IList<TagSummaryDto> tagSummaryDtos = await _tagService.GetAllTagsAsync();

        List<TagListItemViewModel> tagListItemViewModels = _mapper.Map<List<TagListItemViewModel>>(tagSummaryDtos);

        TagListViewModel tagListViewModel = new TagListViewModel
        {
            Tags = tagListItemViewModels
        };

        return OperationResult<TagListViewModel>.Success(tagListViewModel);
    }

    public async Task<OperationResult<TagCreateViewModel>> PrepareCreateViewModelAsync(TagCreateViewModel? tagCreateViewModel = null)
    {
        if (tagCreateViewModel != null)
        {
            await LoadFormReferenceDataAsync(tagCreateViewModel);
            return OperationResult<TagCreateViewModel>.Success(tagCreateViewModel);
        }
        tagCreateViewModel = new TagCreateViewModel();
        await LoadFormReferenceDataAsync(tagCreateViewModel);

        foreach (LanguageLookupViewModel language in tagCreateViewModel.AvailableLanguages)
        {
            tagCreateViewModel.Translations.Add(new TagTranslationViewModel
            {
                LanguageId = language.Id,
                LanguageCode = language.Code
            });
        }

        return OperationResult<TagCreateViewModel>.Success(tagCreateViewModel);
    }

    public async Task<OperationResult<TagEditViewModel>> PrepareEditViewModelAsync(int tagId, TagEditViewModel? tagEditViewModel = null)
    {
        if (tagEditViewModel != null)
        {
            await LoadFormReferenceDataAsync(tagEditViewModel);
            return OperationResult<TagEditViewModel>.Success(tagEditViewModel);
        }

        TagDetailDto? tagUpdateDto = await _tagService.GetTagByIdAsync(tagId);

        if (tagUpdateDto == null)
            return OperationResult<TagEditViewModel>.Failure("Tag not found.", OperationErrorType.NotFound);

        tagEditViewModel = _mapper.Map<TagEditViewModel>(tagUpdateDto);

        await LoadFormReferenceDataAsync(tagEditViewModel);

        foreach (LanguageLookupViewModel language in tagEditViewModel.AvailableLanguages)
        {
            TagTranslationDto? existingTranslationDto = tagUpdateDto.Translations
                .FirstOrDefault(translation => translation.LanguageId == language.Id);

            tagEditViewModel.Translations.Add(new TagTranslationViewModel
            {
                LanguageId = language.Id,
                LanguageCode = language.Code,
                Name = existingTranslationDto?.Name,
                Description = existingTranslationDto?.Description,
            });
        }

        return OperationResult<TagEditViewModel>.Success(tagEditViewModel);
    }

    public async Task<OperationResult> CreateTagAsync(TagCreateViewModel tagCreateViewModel)
    {
        if (tagCreateViewModel == null)
            throw new ArgumentNullException(nameof(tagCreateViewModel));

        TagCreateDto tagCreateDto = new TagCreateDto
        {
            Name = tagCreateViewModel.Name,
            Description = tagCreateViewModel.Description,
            ColorHex = tagCreateViewModel.ColorHex,
            Translations = tagCreateViewModel.Translations.Select(translationViewModel => new TagTranslationDto
            {
                LanguageId = translationViewModel.LanguageId,
                Name = translationViewModel.Name ?? string.Empty,
                Description = translationViewModel.Description,
            }).ToList()
        };

        return await _tagService.CreateTagAsync(tagCreateDto);
    }

    public async Task<OperationResult> UpdateTagAsync(TagEditViewModel tagEditViewModel)
    {
        if (tagEditViewModel == null)
            throw new ArgumentNullException(nameof(tagEditViewModel));

        TagUpdateDto tagUpdateDto = new TagUpdateDto
        {
            Id = tagEditViewModel.Id,
            Name = tagEditViewModel.Name,
            ColorHex = tagEditViewModel.ColorHex,
            Description = tagEditViewModel.Description,
            Translations = tagEditViewModel.Translations.Select(translationViewModel => new TagTranslationDto
            {
                LanguageId = translationViewModel.LanguageId,
                Name = translationViewModel.Name ?? string.Empty,
                Description = translationViewModel.Description,
            }).ToList()
        };

        return await _tagService.UpdateTagAsync(tagUpdateDto);
    }

    public async Task<OperationResult> DeleteTagAsync(int tagId)
    {
        return await _tagService.DeleteTagAsync(tagId);
    }

    private async Task LoadFormReferenceDataAsync(TagFormViewModel tagFormViewModel)
    {
        IList<LanguageLookupDto> publishedLanguages = await _languageService.GetLanguagesLookupAsync();
        tagFormViewModel.AvailableLanguages = _mapper.Map<List<LanguageLookupViewModel>>(publishedLanguages);
    }
}
