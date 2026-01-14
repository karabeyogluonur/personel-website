using Microsoft.EntityFrameworkCore;
using PW.Application.Common.Constants;
using PW.Application.Common.Enums;
using PW.Application.Common.Extensions;
using PW.Application.Interfaces.Content;
using PW.Application.Interfaces.Repositories;
using PW.Application.Interfaces.Storage;
using PW.Application.Models;
using PW.Application.Models.Dtos.Content;
using PW.Domain.Entities;

namespace PW.Services.Content;

public class TechnologyService : ITechnologyService
{
   private readonly IUnitOfWork _unitOfWork;
   private readonly IRepository<Technology> _technologyRepository;
   private readonly IFileProcessorService _fileProcessorService;

   public TechnologyService(IUnitOfWork unitOfWork, IFileProcessorService fileProcessorService)
   {
      _unitOfWork = unitOfWork;
      _technologyRepository = _unitOfWork.GetRepository<Technology>();
      _fileProcessorService = fileProcessorService;
   }

   public async Task<IList<TechnologySummaryDto>> GetAllTechnologiesAsync()
   {
      IList<Technology> technologies = await _technologyRepository.GetAllAsync(
          orderBy: query => query.OrderByDescending(technology => technology.CreatedAt)
      );

      IList<TechnologySummaryDto> result = technologies.Select(technology => new TechnologySummaryDto
      {
         Id = technology.Id,
         Name = technology.Name,
         Description = technology.Description,
         IconImageFileName = technology.IconImageFileName,
         IsActive = technology.IsActive,
         CreatedAt = technology.CreatedAt
      }).ToList();

      return result;
   }

   public async Task<TechnologyDetailDto?> GetTechnologyByIdAsync(int technologyId)
   {
      if (technologyId <= 0) return null;

      Technology technology = await _technologyRepository.GetFirstOrDefaultAsync(
          predicate: technology => technology.Id == technologyId,
          include: source => source.Include(technology => technology.Translations)
      );

      if (technology == null) return null;

      return new TechnologyDetailDto
      {
         Id = technology.Id,
         Name = technology.Name,
         Description = technology.Description ?? string.Empty,
         IsActive = technology.IsActive,
         IconImageFileName = technology.IconImageFileName,
         Translations = technology.Translations.Select(translation => new TechnologyTranslationDto
         {
            LanguageId = translation.LanguageId,
            Name = translation.Name,
            Description = translation.Description
         })
           .ToList()
      };
   }

   public async Task<OperationResult> CreateTechnologyAsync(TechnologyCreateDto technologyCreateDto)
   {
      if (technologyCreateDto == null)
         throw new ArgumentNullException(nameof(technologyCreateDto));

      bool nameExists = await _technologyRepository.ExistsAsync(technology => technology.Name == technologyCreateDto.Name);

      if (nameExists)
         return OperationResult.Failure("Technology name already exists.", OperationErrorType.Conflict);

      string uploadedIconName = string.Empty;

      try
      {
         uploadedIconName = await _fileProcessorService.HandleFileUpdateAsync(
             fileInput: technologyCreateDto.Icon,
             currentDbFileName: null,
             folderPath: StoragePaths.System_Technologies,
             slugName: technologyCreateDto.Name
         );

         Technology technology = new Technology
         {
            Name = technologyCreateDto.Name,
            Description = technologyCreateDto.Description,
            IsActive = technologyCreateDto.IsActive,
            IconImageFileName = uploadedIconName,
            CreatedAt = DateTime.UtcNow,
            Translations = new List<TechnologyTranslation>()
         };

         technology.Translations.SyncTranslations(
             translationDtos: technologyCreateDto.Translations,
             isEmptyPredicate: (TechnologyTranslationDto translationDto) =>
                 string.IsNullOrWhiteSpace(translationDto.Name) &&
                 string.IsNullOrWhiteSpace(translationDto.Description),
             mapAction: (TechnologyTranslation translation, TechnologyTranslationDto translationDto) =>
             {
                translation.Name = translationDto.Name;
                translation.Description = translationDto.Description;
             }
         );

         await _technologyRepository.InsertAsync(technology);
         await _unitOfWork.CommitAsync();

         return OperationResult.Success();
      }
      catch (Exception)
      {
         if (!string.IsNullOrEmpty(uploadedIconName))
            await _fileProcessorService.DeleteFileAsync(StoragePaths.System_Technologies, uploadedIconName);

         return OperationResult.Failure("Failed to create technology.", OperationErrorType.Technical);
      }
   }

   public async Task<OperationResult> UpdateTechnologyAsync(TechnologyUpdateDto technologyUpdateDto)
   {
      if (technologyUpdateDto == null)
         throw new ArgumentNullException(nameof(technologyUpdateDto));

      Technology technology = await _technologyRepository.GetFirstOrDefaultAsync(
          predicate: technology => technology.Id == technologyUpdateDto.Id,
          include: source => source.Include(technology => technology.Translations),
          disableTracking: false
      );

      if (technology == null)
         return OperationResult.Failure("Technology not found.", OperationErrorType.NotFound);

      if (technology.Name != technologyUpdateDto.Name)
      {
         bool nameExists = await _technologyRepository.ExistsAsync(technology => technology.Name == technologyUpdateDto.Name);

         if (nameExists)
            return OperationResult.Failure("Technology name already exists.", OperationErrorType.Conflict);
      }

      technology.IconImageFileName = await _fileProcessorService.HandleFileUpdateAsync(
          fileInput: technologyUpdateDto.Icon,
          currentDbFileName: technology.IconImageFileName,
          folderPath: StoragePaths.System_Technologies,
          slugName: technologyUpdateDto.Name
      );

      technology.Name = technologyUpdateDto.Name;
      technology.Description = technologyUpdateDto.Description;
      technology.IsActive = technologyUpdateDto.IsActive;
      technology.UpdatedAt = DateTime.UtcNow;

      technology.Translations.SyncTranslations(
          translationDtos: technologyUpdateDto.Translations,
          isEmptyPredicate: (TechnologyTranslationDto translationDto) =>
              string.IsNullOrWhiteSpace(translationDto.Name) &&
              string.IsNullOrWhiteSpace(translationDto.Description),
          mapAction: (TechnologyTranslation translation, TechnologyTranslationDto translationDto) =>
          {
             translation.Name = translationDto.Name;
             translation.Description = translationDto.Description;
          }
      );

      await _unitOfWork.CommitAsync();

      return OperationResult.Success();
   }

   public async Task<OperationResult> DeleteTechnologyAsync(int technologyId)
   {
      Technology technology = await _technologyRepository.GetFirstOrDefaultAsync(predicate: technology => technology.Id == technologyId);

      if (technology == null)
         return OperationResult.Failure("Technology not found.", OperationErrorType.NotFound);

      await _fileProcessorService.DeleteFileAsync(StoragePaths.System_Technologies, technology.IconImageFileName);

      _technologyRepository.Delete(technology);
      await _unitOfWork.CommitAsync();

      return OperationResult.Success();
   }
}
