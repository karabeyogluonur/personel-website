namespace PW.Application.Common.Dtos;

public class FileUploadDto
{
   public Stream? FileStream { get; set; }
   public string? FileName { get; set; }
   public bool IsRemoveRequested { get; set; }

   public FileUploadDto()
   {
   }

   public FileUploadDto(Stream? fileStream, string? fileName, bool isRemoveRequested)
   {
      FileStream = fileStream;
      FileName = fileName;
      IsRemoveRequested = isRemoveRequested;
   }

   public bool HasValidFile => FileStream != null && FileStream.Length > 0 && !string.IsNullOrWhiteSpace(FileName);
}
