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
        public IQueryable<Language> GetAllPublishedLanguages()
        {
            return _languageRepository.GetAll(predicate: language => language.IsPublished);
        }
    }
}
