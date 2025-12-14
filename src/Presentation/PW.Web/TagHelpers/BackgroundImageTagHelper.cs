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
            string imageUrl;

            if (!string.IsNullOrEmpty(FileName) && !string.IsNullOrEmpty(Folder))
            {
                string cleanFolder = Folder.Trim('/');
                imageUrl = $"/uploads/{cleanFolder}/{FileName}";
            }
            else
            {
                imageUrl = DefaultSrc ?? "/assets/media/misc/image-placeholder.png";
            }

            var styleAttribute = output.Attributes["style"];
            string existingStyle = styleAttribute?.Value.ToString() ?? "";

            string bgStyle = $"background-image: url('{imageUrl}');";

            if (string.IsNullOrEmpty(existingStyle))
            {
                output.Attributes.SetAttribute("style", bgStyle);
            }
            else
            {
                string separator = existingStyle.TrimEnd().EndsWith(";") ? " " : "; ";
                output.Attributes.SetAttribute("style", $"{existingStyle}{separator}{bgStyle}");
            }
        }
    }
}
