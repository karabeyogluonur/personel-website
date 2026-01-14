using PW.Application.Features.Localization.Dtos;

namespace PW.Application.Common.Interfaces;

public interface IWorkContext
{
   Task<LanguageDetailDto> GetCurrentLanguageAsync();
   Task SetCurrentLanguageAsync(LanguageDetailDto languageDetailDto);
}
