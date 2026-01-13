using PW.Application.Models;
using PW.Application.Models.Dtos.Storages;
using PW.Domain.Entities;

namespace PW.Application.Interfaces.Storage;

public interface IAssetService
{
    Task<OperationResult<Asset>> UploadAsync(AssetUploadDto assetUploadDto);

    Task<OperationResult> DeleteAsync(int assetId);
}
