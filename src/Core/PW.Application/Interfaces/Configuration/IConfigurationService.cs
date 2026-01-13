using PW.Application.Models;
using PW.Application.Models.Dtos.Configurations;

namespace PW.Application.Interfaces.Configuration;

public interface IConfigurationService
{
    Task<GeneralSettingsDto> GetGeneralSettingsAsync();
    Task<OperationResult> UpdateGeneralSettingsAsync(GeneralSettingsUpdateDto generalSettingsUpdateDto);

    Task<ProfileSettingsDto> GetProfileSettingsAsync();
    Task<OperationResult> UpdateProfileSettingsAsync(ProfileSettingsUpdateDto profileSettingsUpdateDto);
}
