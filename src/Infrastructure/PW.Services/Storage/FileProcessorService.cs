using PW.Application.Common.Enums;
using PW.Application.Interfaces.Storage;
using PW.Application.Models.Dtos.Common;

namespace PW.Services.Storage;

public class FileProcessorService : IFileProcessorService
{
   private readonly IStorageService _storageService;

   public FileProcessorService(IStorageService storageService)
   {
      _storageService = storageService;
   }

   public async Task<string> HandleFileUpdateAsync(
        FileUploadDto fileInput,
        string? currentDbFileName,
        string folderPath,
        string slugName,
        FileNamingMode mode = FileNamingMode.Unique)
   {
      if (fileInput == null) throw new ArgumentNullException(nameof(fileInput));

      if (fileInput.HasValidFile)
      {
         await DeleteFileAsync(folderPath, currentDbFileName);

         return await _storageService.UploadAsync(
             fileStream: fileInput.FileStream!,
             fileName: fileInput.FileName!,
             folder: folderPath,
             mode: mode,
             customName: slugName
         );
      }

      if (fileInput.IsRemoveRequested)
      {
         await DeleteFileAsync(folderPath, currentDbFileName);
         return string.Empty;
      }
      if (!string.IsNullOrEmpty(currentDbFileName))
      {
         if (mode == FileNamingMode.Specific)
         {
            string extension = Path.GetExtension(currentDbFileName);
            string expectedFileName = $"{slugName}{extension}";

            if (!currentDbFileName.Equals(expectedFileName, StringComparison.OrdinalIgnoreCase))
               return await RenameFileAsync(folderPath, currentDbFileName, slugName);
         }
      }

      return currentDbFileName ?? string.Empty;
   }

   public async Task DeleteFileAsync(string folderPath, string? fileName)
   {
      if (!string.IsNullOrEmpty(fileName))
         await _storageService.DeleteAsync(folderPath, fileName);
   }

   public async Task<string> RenameFileAsync(string folderPath, string currentFileName, string newSlug)
   {
      if (string.IsNullOrEmpty(currentFileName)) return string.Empty;

      string extension = Path.GetExtension(currentFileName);
      string newFileName = $"{newSlug}{extension}";

      if (currentFileName.Equals(newFileName, StringComparison.OrdinalIgnoreCase))
         return currentFileName;

      await _storageService.RenameAsync(folderPath, currentFileName, newFileName);

      return newFileName;
   }

   public async Task ProcessFileActionAsync(
       FileUploadDto fileInput,
       string folderPath,
       string slugName,
       Func<Task<string>> getCurrentFileNameAction,
       Func<string, Task> saveNewFileNameAction,
       FileNamingMode mode = FileNamingMode.Unique)
   {
      string currentFileName = await getCurrentFileNameAction();

      string newFileName = await HandleFileUpdateAsync(fileInput, currentFileName, folderPath, slugName, mode);

      if (currentFileName != newFileName)
         await saveNewFileNameAction(newFileName);
   }
}
