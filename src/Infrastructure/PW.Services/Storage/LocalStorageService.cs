using Microsoft.AspNetCore.Http;
using PW.Application.Interfaces.Storage;
using Microsoft.AspNetCore.Hosting;


namespace PW.Services.Storage
{
    public class LocalStorageService : IStorageService
    {
        private readonly IWebHostEnvironment _env;
        private const string RootContainerName = "uploads";

        public LocalStorageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task UploadAsync(IFormFile file, string folder, string fileName)
        {
            var path = Path.Combine(_env.WebRootPath, RootContainerName, folder);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var fullPath = Path.Combine(path, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);
        }

        public Task DeleteAsync(string folder, string fileName)
        {
            var fullPath = Path.Combine(_env.WebRootPath, RootContainerName, folder, fileName);

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            return Task.CompletedTask;
        }

        public Task RenameAsync(string folder, string oldFileName, string newFileName)
        {
            var path = Path.Combine(_env.WebRootPath, RootContainerName, folder);
            string oldPath = Path.Combine(path, oldFileName);
            string newPath = Path.Combine(path, newFileName);

            if (File.Exists(oldPath))
                File.Move(oldPath, newPath);

            return Task.CompletedTask;
        }

        public string GetUrl(string folder, string fileName)
        {
            return $"/{RootContainerName}/{folder}/{fileName}".Replace("\\", "/");
        }
    }
}
