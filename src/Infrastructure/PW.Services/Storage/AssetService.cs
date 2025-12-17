using Microsoft.AspNetCore.Http;
using PW.Application.Common.Enums;
using PW.Application.Common.Extensions;
using PW.Application.Interfaces.Repositories;
using PW.Application.Interfaces.Storage;
using PW.Domain.Entities;

namespace PW.Services.Storage
{
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

        public async Task<Asset> UploadAsync(IFormFile file, string folder, string seoTitle, string? altText = null)
        {
            string savedFileName = await _storageService.UploadAsync(
                file,
                folder,
                FileNamingMode.Unique,
                customName: seoTitle
            );

            var asset = new Asset
            {
                FileName = savedFileName,
                Folder = folder,
                Extension = file.GetExtension(),
                ContentType = file.ContentType,
                SizeBytes = file.Length,
                AltText = !string.IsNullOrEmpty(altText) ? altText : seoTitle
            };

            await _assetRepository.InsertAsync(asset);
            await _unitOfWork.CommitAsync();

            return asset;
        }

        public async Task DeleteAsync(int assetId)
        {
            var asset = await _assetRepository.FindAsync(assetId);
            if (asset == null) return;

            await _storageService.DeleteAsync(asset.Folder, asset.FileName);

            _assetRepository.Delete(asset);
            await _unitOfWork.CommitAsync();
        }
    }
}
