using PW.Application.Common.Enums;
using PW.Application.Interfaces.Content;
using PW.Application.Interfaces.Repositories;
using PW.Application.Models;
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

        public async Task<Technology> GetTechnologyByIdAsync(int technologyId)
        {
            if (technologyId <= 0) return null;
            return await _technologyRepository.FindAsync(technologyId);
        }

        public async Task<IList<Technology>> GetAllTechnologiesAsync()
        {
            return await _technologyRepository.GetAllAsync();
        }

        public async Task<OperationResult> InsertTechnologyAsync(Technology technology)
        {
            if (technology is null)
                throw new ArgumentNullException(nameof(technology));

            bool technologyExists = await _technologyRepository.ExistsAsync(t => t.Name == technology.Name);

            if (technologyExists)
                return OperationResult.Failure("Technology name exists.", OperationErrorType.Conflict);

            try
            {
                await _technologyRepository.InsertAsync(technology);
                await _unitOfWork.CommitAsync();

                return OperationResult.Success();
            }
            catch (Exception)
            {
                return OperationResult.Failure("Failed to create technology.", OperationErrorType.Technical);
            }
        }

        public async Task<OperationResult> UpdateTechnologyAsync(Technology technology)
        {
            if (technology is null)
                throw new ArgumentNullException(nameof(technology));

            Technology existingTechnology = await _technologyRepository.FindAsync(technology.Id);

            if (existingTechnology is null)
                return OperationResult.Failure("Technology not found.", OperationErrorType.NotFound);

            try
            {
                _technologyRepository.Update(technology);
                await _unitOfWork.CommitAsync();

                return OperationResult.Success();
            }
            catch (Exception)
            {
                return OperationResult.Failure("Failed to update technology.", OperationErrorType.Technical);
            }
        }

        public async Task<OperationResult> DeleteTechnologyAsync(Technology technology)
        {
            if (technology is null)
                throw new ArgumentNullException(nameof(technology));

            // Business Rule: Check if used in any Project?
            // if (technology.Projects.Any()) return OperationResult.Failure("Cannot delete used technology.", OperationErrorType.BusinessRule);

            try
            {
                _technologyRepository.Delete(technology);
                await _unitOfWork.CommitAsync();

                return OperationResult.Success();
            }
            catch (Exception)
            {
                return OperationResult.Failure("Failed to delete technology.", OperationErrorType.Technical);
            }
        }
    }
}
