using System.Threading.Tasks;
using PW.Application.Interfaces.Localization;
using PW.Application.Interfaces.Repositories;
using PW.Domain.Entities;

namespace PW.Services.Localization
{
    public class LanguageService : ILanguageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Language> _languageRepository;

        public LanguageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _languageRepository = _unitOfWork.GetRepository<Language>();
        }

        public Task<IList<Language>> GetAllLanguagesAsync()
        {
            return _languageRepository.GetAllAsync();
        }

        public async Task InsertLanguageAsync(Language language)
        {
            if (language.IsDefault)
            {
                IList<Language> defaultLanguages = await _languageRepository.GetAllAsync(predicate: x => x.IsDefault);

                foreach (var defaultLanguage in defaultLanguages)
                    defaultLanguage.IsDefault = false;

                _languageRepository.Update(defaultLanguages);
            }

            await _languageRepository.InsertAsync(language);
            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateLanguageAsync(Language language)
        {
            if (language.IsDefault)
            {
                IList<Language> defaultLanguages = await _languageRepository.GetAllAsync(predicate: x => x.IsDefault);

                foreach (var defaultLanguage in defaultLanguages)
                    defaultLanguage.IsDefault = false;

                _languageRepository.Update(defaultLanguages);
            }

            _languageRepository.Update(language);
            await _unitOfWork.CommitAsync();
        }

        public IQueryable<Language> GetAllPublishedLanguages()
        {
            return _languageRepository.GetAll(predicate: language => language.IsPublished);
        }
        public async Task<IList<Language>> GetAllPublishedLanguagesAsync()
        {
            return await _languageRepository.GetAllAsync(predicate: language => language.IsPublished);
        }
        public async Task<Language> GetLanguageByCodeAsync(string code)
        {
            return await _languageRepository.GetFirstOrDefaultAsync(predicate: language => language.Code == code);
        }

        public async Task<Language> GetLanguageByIdAsync(int id)
        {
            return await _languageRepository.GetFirstOrDefaultAsync(predicate: language => language.Id == id);
        }
        public async Task DeleteLanguageAsync(Language language)
        {
            _languageRepository.Delete(language);
            await _unitOfWork.CommitAsync();
        }
    }
}
