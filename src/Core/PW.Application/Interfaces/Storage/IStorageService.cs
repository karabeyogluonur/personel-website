using Microsoft.AspNetCore.Http;

namespace PW.Application.Interfaces.Storage
{
    public interface IStorageService
    {

        Task UploadAsync(IFormFile file, string folder, string fileName);
        Task DeleteAsync(string folder, string fileName);
        Task RenameAsync(string folder, string oldFileName, string newFileName);
        string GetUrl(string folder, string fileName);
    }
}
