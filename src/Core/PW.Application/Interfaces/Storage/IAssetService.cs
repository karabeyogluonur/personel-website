using Microsoft.AspNetCore.Http;
using PW.Domain.Entities;

namespace PW.Application.Interfaces.Storage
{
    public interface IAssetService
    {
        Task<Asset> UploadAsync(IFormFile file, string folder, string seoTitle, string altText = null);

        Task DeleteAsync(int assetId);
    }
}
