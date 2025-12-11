using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using System.Text;
using PW.Application.Interfaces.Storage;
using PW.Application.Interfaces.Repositories;
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

        public async Task<Asset> UploadAsync(IFormFile file, string folder, string seoTitle, string altText = null)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Dosya boş olamaz.");

            string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            string safeTitle = GenerateSlug(seoTitle);
            string uniqueSuffix = Guid.NewGuid().ToString().Substring(0, 8);

            string fileName = $"{safeTitle}-{uniqueSuffix}{fileExtension}";

            await _storageService.UploadAsync(file, folder, fileName);

            var asset = new Asset
            {
                FileName = fileName,
                Folder = folder,
                Extension = fileExtension,
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

        private string GenerateSlug(string phrase)
        {
            if (string.IsNullOrEmpty(phrase)) return "file";

            string str = phrase.ToLowerInvariant();

            str = str.Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u")
                     .Replace("ş", "s").Replace("ö", "o").Replace("ç", "c");

            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");

            str = Regex.Replace(str, @"\s+", "-").Trim();

            str = Regex.Replace(str, @"-+", "-");

            return str;
        }
    }
}
