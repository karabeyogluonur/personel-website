using PW.Application.Common.Enums;
using PW.Application.Models.Dtos.Common;

namespace PW.Application.Interfaces.Storage;

public interface IFileProcessorService
{
   Task<string> HandleFileUpdateAsync(
        FileUploadDto fileInput,
        string? currentDbFileName,
        string folderPath,
        string slugName,
        FileNamingMode mode = FileNamingMode.Unique
    );

   Task DeleteFileAsync(string folderPath, string? fileName);

   Task<string> RenameFileAsync(string folderPath, string currentFileName, string newSlug);

   Task ProcessFileActionAsync(
       FileUploadDto fileInput,
       string folderPath,
       string slugName,
       Func<Task<string>> getCurrentFileNameAction,
       Func<string, Task> saveNewFileNameAction,
       FileNamingMode mode = FileNamingMode.Unique
   );
}
