using PW.Application.Common.Enums;
using PW.Application.Interfaces.Repositories;
using PW.Application.Interfaces.Storage;
using PW.Application.Models;
using PW.Application.Models.Dtos.Storages;
using PW.Domain.Entities;

namespace PW.Services.Storage;

public class AssetService : IAssetService
{
    private readonly IStorageService _storageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Asset> _assetRepository;

    public AssetService(IStorageService storageService, IUnitOfWork unitOfWork)
    {
        _storageService = storageService;
        _unitOfWork = unitOfWork;
        _assetRepository = _unitOfWork.GetRepository<Asset>();
    }

    public async Task<OperationResult<Asset>> UploadAsync(AssetUploadDto assetUploadDto)
    {
        if (assetUploadDto == null)
            throw new ArgumentNullException(nameof(assetUploadDto));

        if (assetUploadDto.FileStream == null || assetUploadDto.FileStream.Length == 0)
            return OperationResult<Asset>.Failure("File stream is empty.", OperationErrorType.ValidationError);

        string savedFileName = await _storageService.UploadAsync(
            fileStream: assetUploadDto.FileStream,
            fileName: assetUploadDto.FileName,
            folder: assetUploadDto.Folder,
            mode: FileNamingMode.Unique,
            customName: assetUploadDto.SeoTitle
        );

        Asset asset = new Asset
        {
            FileName = savedFileName,
            Folder = assetUploadDto.Folder,
            Extension = Path.GetExtension(assetUploadDto.FileName).ToLowerInvariant(),
            ContentType = assetUploadDto.ContentType,
            AltText = !string.IsNullOrEmpty(assetUploadDto.AltText) ? assetUploadDto.AltText : assetUploadDto.SeoTitle,
        };

        foreach (AssetTranslationDto translationDto in assetUploadDto.Translations)
        {
            asset.Translations.Add(new AssetTranslation
            {
                LanguageId = translationDto.LanguageId,
                AltText = translationDto.AltText
            });
        }

        try
        {
            await _assetRepository.InsertAsync(asset);
            await _unitOfWork.CommitAsync();

            return OperationResult<Asset>.Success(asset);
        }
        catch (Exception)
        {
            await _storageService.DeleteAsync(asset.Folder, asset.FileName);
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
            await _storageService.DeleteAsync(asset.Folder, asset.FileName);
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
