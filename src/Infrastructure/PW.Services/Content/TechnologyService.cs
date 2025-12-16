using PW.Application.Interfaces.Content;
using PW.Application.Interfaces.Repositories;
using PW.Domain.Entities;

namespace PW.Services.Content
{
    public class TechnologyService : ITechnologyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Technology> _technologyRepository;

        public TechnologyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _technologyRepository = _unitOfWork.GetRepository<Technology>();
        }

        public async Task DeleteTechnologyAsync(Technology technology)
        {
            _technologyRepository.Delete(technology);

            await _unitOfWork.CommitAsync();
        }

        public async Task<IList<Technology>> GetAllTechnologiesAsync()
        {
            return await _technologyRepository.GetAllAsync();
        }

        public async Task<Technology> GetTechnologyByIdAsync(int technologyId)
        {
            return await _technologyRepository.FindAsync(technologyId);
        }

        public async Task InsertTechnologyAsync(Technology technology)
        {
            await _technologyRepository.InsertAsync(technology);
            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateTechnologyAsync(Technology technology)
        {
            _technologyRepository.Update(technology);
            await _unitOfWork.CommitAsync();
        }
    }
}
