using Microsoft.AspNetCore.Hosting;
using PW.Application.Common.Enums;
using PW.Application.Common.Extensions;
using PW.Application.Interfaces.Storage;

namespace PW.Storage.Services;

public class LocalStorageService : IStorageService
{
   private readonly IWebHostEnvironment _webHostEnvironment;
   private const string RootContainerName = "uploads";

   public LocalStorageService(IWebHostEnvironment webHostEnvironment)
   {
      _webHostEnvironment = webHostEnvironment;
   }

   public async Task<string> UploadAsync(Stream fileStream, string fileName, string folder, FileNamingMode mode = FileNamingMode.Unique, string? customName = null)
   {
      if (fileStream is null || fileStream.Length is 0)
         throw new ArgumentException("File stream cannot be empty.", nameof(fileStream));

      if (string.IsNullOrEmpty(fileName))
         throw new ArgumentException("Filename cannot be empty.", nameof(fileName));

      string webRootPath = _webHostEnvironment.WebRootPath;
      string folderPath = Path.Combine(webRootPath, RootContainerName, folder);

      if (!Directory.Exists(folderPath))
         Directory.CreateDirectory(folderPath);

      string generatedFileName = GenerateFileName(fileName, mode, customName);
      string fullPath = Path.Combine(folderPath, generatedFileName);

      if (fileStream.CanSeek)
         fileStream.Position = 0;

      using FileStream outputFileStream = new FileStream(fullPath, FileMode.Create);
      await fileStream.CopyToAsync(outputFileStream);

      return generatedFileName;
   }

   private string GenerateFileName(string originalFileName, FileNamingMode mode, string? customName)
   {
      string extension = Path.GetExtension(originalFileName);

      string baseName = !string.IsNullOrEmpty(customName)
          ? customName.ToUrlSlug()
          : Path.GetFileNameWithoutExtension(originalFileName).ToUrlSlug();

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
      string webRootPath = _webHostEnvironment.WebRootPath;
      string fullPath = Path.Combine(webRootPath, RootContainerName, folder, fileName);

      if (File.Exists(fullPath))
         File.Delete(fullPath);

      return Task.CompletedTask;
   }

   public Task RenameAsync(string folder, string oldFileName, string newFileName)
   {
      string webRootPath = _webHostEnvironment.WebRootPath;
      string folderPath = Path.Combine(webRootPath, RootContainerName, folder);

      string oldPath = Path.Combine(folderPath, oldFileName);
      string newPath = Path.Combine(folderPath, newFileName);

      if (File.Exists(oldPath))
         File.Move(oldPath, newPath);

      return Task.CompletedTask;
   }

   public string GetUrl(string folder, string fileName)
   {
      return $"/{RootContainerName}/{folder}/{fileName}".Replace("\\", "/");
   }
}
