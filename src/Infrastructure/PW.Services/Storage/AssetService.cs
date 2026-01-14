using PW.Application.Common.Enums;
using PW.Application.Common.Extensions;
using PW.Application.Interfaces.Repositories;
using PW.Application.Interfaces.Storage;
using PW.Application.Models;
using PW.Application.Models.Dtos.Storages;
using PW.Domain.Entities;

namespace PW.Services.Storage;

public class AssetService : IAssetService
{
   private readonly IFileProcessorService _fileProcessorService;
   private readonly IUnitOfWork _unitOfWork;
   private readonly IRepository<Asset> _assetRepository;

   public AssetService(IFileProcessorService fileProcessorService, IUnitOfWork unitOfWork)
   {
      _fileProcessorService = fileProcessorService;
      _unitOfWork = unitOfWork;
      _assetRepository = _unitOfWork.GetRepository<Asset>();
   }

   public async Task<OperationResult<Asset>> UploadAsync(AssetUploadDto assetUploadDto)
   {
      if (assetUploadDto == null)
         throw new ArgumentNullException(nameof(assetUploadDto));

      if (!assetUploadDto.File.HasValidFile)
         return OperationResult<Asset>.Failure("File stream is empty.", OperationErrorType.ValidationError);

      string savedFileName = string.Empty;

      try
      {
         savedFileName = await _fileProcessorService.HandleFileUpdateAsync(
             fileInput: assetUploadDto.File,
             currentDbFileName: null,
             folderPath: assetUploadDto.Folder,
             slugName: assetUploadDto.SeoTitle,
             mode: FileNamingMode.Unique
         );

         Asset asset = new Asset
         {
            FileName = savedFileName,
            Folder = assetUploadDto.Folder,
            Extension = Path.GetExtension(assetUploadDto.File.FileName).ToLowerInvariant(),
            ContentType = assetUploadDto.ContentType,
            AltText = !string.IsNullOrEmpty(assetUploadDto.AltText) ? assetUploadDto.AltText : assetUploadDto.SeoTitle,
            Translations = new List<AssetTranslation>()
         };

         asset.Translations.SyncTranslations(
             translationDtos: assetUploadDto.Translations,
             isEmptyPredicate: (AssetTranslationDto dto) => string.IsNullOrWhiteSpace(dto.AltText),
             mapAction: (AssetTranslation translation, AssetTranslationDto dto) =>
             {
                translation.AltText = dto.AltText;
             }
         );

         await _assetRepository.InsertAsync(asset);
         await _unitOfWork.CommitAsync();

         return OperationResult<Asset>.Success(asset);
      }
      catch (Exception)
      {
         if (!string.IsNullOrEmpty(savedFileName))
            await _fileProcessorService.DeleteFileAsync(assetUploadDto.Folder, savedFileName);

         return OperationResult<Asset>.Failure("Failed to save asset record to database.", OperationErrorType.Technical);
      }
   }

   public async Task<OperationResult> DeleteAsync(int assetId)
   {
      Asset asset = await _assetRepository.GetFirstOrDefaultAsync(
          predicate: asset => asset.Id == assetId,
          disableTracking: false
      );

      if (asset == null)
         return OperationResult.Failure("Asset not found.", OperationErrorType.NotFound);

      try
      {
         await _fileProcessorService.DeleteFileAsync(asset.Folder, asset.FileName);

         _assetRepository.Delete(asset);
         await _unitOfWork.CommitAsync();

         return OperationResult.Success();
      }
      catch (Exception)
      {
         return OperationResult.Failure("Failed to delete asset.", OperationErrorType.Technical);
      }
   }
}
