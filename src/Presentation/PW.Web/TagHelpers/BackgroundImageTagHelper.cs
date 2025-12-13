using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PW.Web.TagHelpers
{
    [HtmlTargetElement("div", Attributes = "bg-file-name")]
    public class BackgroundImageTagHelper : TagHelper
    {
        [HtmlAttributeName("bg-file-name")]
        public string? FileName { get; set; }

        [HtmlAttributeName("bg-folder")]
        public string? Folder { get; set; }

        [HtmlAttributeName("bg-default-src")]
        public string? DefaultSrc { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // 1. Resim yolunu belirle
            string imageUrl;

            if (!string.IsNullOrEmpty(FileName) && !string.IsNullOrEmpty(Folder))
            {
                // Url oluşturma mantığı (Storage yapına göre burayı özelleştirebilirsin)
                // Örn: /uploads/system/profiles/avatar.jpg
                // Başında '/' olup olmadığını kontrol edip ekliyoruz.
                string cleanFolder = Folder.Trim('/');
                imageUrl = $"/uploads/{cleanFolder}/{FileName}";
            }
            else
            {
                // Resim yoksa varsayılanı kullan
                imageUrl = DefaultSrc ?? "/assets/media/misc/image-placeholder.png";
            }

            // 2. Mevcut style attribute'unu al
            var styleAttribute = output.Attributes["style"];
            string existingStyle = styleAttribute?.Value.ToString() ?? "";

            // 3. background-image stilini oluştur
            string bgStyle = $"background-image: url('{imageUrl}');";

            // 4. Stilleri birleştir (Mevcut stilleri ezmemek için sona ekliyoruz)
            // Eğer style attribute yoksa oluştur, varsa sonuna ekle.
            if (string.IsNullOrEmpty(existingStyle))
            {
                output.Attributes.SetAttribute("style", bgStyle);
            }
            else
            {
                // Sonunda noktalı virgül var mı kontrol et
                string separator = existingStyle.TrimEnd().EndsWith(";") ? " " : "; ";
                output.Attributes.SetAttribute("style", $"{existingStyle}{separator}{bgStyle}");
            }
        }
    }
}
