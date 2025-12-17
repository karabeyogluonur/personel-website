using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PW.Application.Common.Enums;
using PW.Application.Common.Extensions;
using PW.Application.Interfaces.Storage;

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

        public async Task<string> UploadAsync(IFormFile file, string folder, FileNamingMode mode = FileNamingMode.Unique, string? customName = null)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File cannot be empty.", nameof(file));

            var path = Path.Combine(_env.WebRootPath, RootContainerName, folder);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string fileName = GenerateFileName(file, mode, customName);

            var fullPath = Path.Combine(path, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return fileName;
        }

        private string GenerateFileName(IFormFile file, FileNamingMode mode, string? customName)
        {
            string extension = file.GetExtension();

            string baseName = !string.IsNullOrEmpty(customName)
                ? customName.ToUrlSlug()
                : Path.GetFileNameWithoutExtension(file.FileName).ToUrlSlug();

            switch (mode)
            {
                case FileNamingMode.Specific:
                    return $"{baseName}{extension}";

                case FileNamingMode.Unique:
                default:
                    string uniqueSuffix = Guid.NewGuid().ToString("N").Substring(0, 8);
                    return $"{baseName}-{uniqueSuffix}{extension}";
            }
        }

        public Task DeleteAsync(string folder, string fileName)
        {
            var fullPath = Path.Combine(_env.WebRootPath, RootContainerName, folder, fileName);
            if (File.Exists(fullPath)) File.Delete(fullPath);
            return Task.CompletedTask;
        }

        public Task RenameAsync(string folder, string oldFileName, string newFileName)
        {
            var path = Path.Combine(_env.WebRootPath, RootContainerName, folder);
            var oldPath = Path.Combine(path, oldFileName);
            var newPath = Path.Combine(path, newFileName);
            if (File.Exists(oldPath)) File.Move(oldPath, newPath);
            return Task.CompletedTask;
        }

        public string GetUrl(string folder, string fileName)
        {
            return $"/{RootContainerName}/{folder}/{fileName}".Replace("\\", "/");
        }
    }
}
