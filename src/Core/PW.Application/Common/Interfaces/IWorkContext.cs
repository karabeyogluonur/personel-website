using PW.Application.Models.Dtos.Localization;

namespace PW.Application.Common.Interfaces;

public interface IWorkContext
{
   Task<LanguageDetailDto> GetCurrentLanguageAsync();
   Task SetCurrentLanguageAsync(LanguageDetailDto languageDetailDto);
}
