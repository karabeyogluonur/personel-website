using PW.Application.Features.Assets.Dtos;
using PW.Application.Utilities.Results;
using PW.Domain.Entities;

namespace PW.Application.Features.Assets;

public interface IAssetService
{
   Task<OperationResult<Asset>> UploadAsync(AssetUploadDto assetUploadDto);

   Task<OperationResult> DeleteAsync(int assetId);
}
