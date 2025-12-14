using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PW.Web.TagHelpers
{
    [HtmlTargetElement("link", Attributes = "icon-file-name")]
    public class FaviconTagHelper : TagHelper
    {
        [HtmlAttributeName("icon-file-name")]
        public string? FileName { get; set; }

        [HtmlAttributeName("icon-folder")]
        public string? Folder { get; set; }

        [HtmlAttributeName("icon-default-src")]
        public string? DefaultSrc { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string iconUrl;

            if (!string.IsNullOrEmpty(FileName) && !string.IsNullOrEmpty(Folder))
            {
                string cleanFolder = Folder.Trim('/');
                iconUrl = $"/uploads/{cleanFolder}/{FileName}";
            }
            else
            {
                iconUrl = DefaultSrc ?? "/themes/metronic/assets/media/logos/favicon-black.ico";
            }

            if (!output.Attributes.ContainsName("rel"))
            {
                output.Attributes.SetAttribute("rel", "icon");
            }

            output.Attributes.SetAttribute("href", iconUrl);
        }
    }
}
