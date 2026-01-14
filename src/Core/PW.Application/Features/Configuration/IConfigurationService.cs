using PW.Application.Features.Configuration.Dtos;
using PW.Application.Utilities.Results;

namespace PW.Application.Features.Configuration;

public interface IConfigurationService
{
   Task<GeneralSettingsDto> GetGeneralSettingsAsync();
   Task<OperationResult> UpdateGeneralSettingsAsync(GeneralSettingsUpdateDto generalSettingsUpdateDto);

   Task<ProfileSettingsDto> GetProfileSettingsAsync();
   Task<OperationResult> UpdateProfileSettingsAsync(ProfileSettingsUpdateDto profileSettingsUpdateDto);
}
