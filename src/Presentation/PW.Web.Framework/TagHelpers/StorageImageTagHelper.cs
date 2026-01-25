using Microsoft.AspNetCore.Razor.TagHelpers;
using PW.Application.Interfaces.Storage;

namespace PW.Web.Framework.TagHelpers;

[HtmlTargetElement("img", Attributes = "file-name, folder")]
public class StorageImageTagHelper : TagHelper
{
   private readonly IStorageService _storageService;

   public StorageImageTagHelper(IStorageService storageService)
   {
      _storageService = storageService;
   }

   [HtmlAttributeName("file-name")]
   public string? FileName { get; set; }

   [HtmlAttributeName("folder")]
   public string? Folder { get; set; }

   [HtmlAttributeName("default-src")]
   public string? DefaultSrc { get; set; }

   public override void Process(TagHelperContext context, TagHelperOutput output)
   {
      if (string.IsNullOrEmpty(FileName))
      {
         if (!string.IsNullOrEmpty(DefaultSrc))
            output.Attributes.SetAttribute("src", DefaultSrc);
         else
            output.SuppressOutput();
         return;
      }

      if (string.IsNullOrEmpty(Folder))
         return;

      string url = _storageService.GetUrl(Folder, FileName);

      output.Attributes.SetAttribute("src", url);
   }
}
