using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PW.Web.Framework.TagHelpers;

[HtmlTargetElement("validation-alert")]
public class ValidationAlertTagHelper : TagHelper
{
   [ViewContext]
   [HtmlAttributeNotBound]
   public ViewContext ViewContext { get; set; } = default!;
   public override void Process(TagHelperContext context, TagHelperOutput output)
   {
      if (ViewContext.ViewData.ModelState.IsValid)
      {
         output.SuppressOutput();
         return;
      }
      var errors = ViewContext.ViewData.ModelState.Values
          .SelectMany(v => v.Errors)
          .Select(e => e.ErrorMessage)
          .ToList();
      if (!errors.Any())
      {
         output.SuppressOutput();
         return;
      }
      output.TagName = "div";
      output.TagMode = TagMode.StartTagAndEndTag;
      output.Attributes.SetAttribute("class", "alert alert-dismissible bg-danger d-flex flex-column flex-sm-row p-5");
      output.Attributes.SetAttribute("role", "alert");
      var sb = new StringBuilder();
      sb.Append(@"<div class=""d-flex flex-column text-light pe-0 pe-sm-10"">");
      sb.Append(@"<ul class=""mb-0"">");
      foreach (var error in errors)
      {
         sb.Append($@"<li>{HtmlEncoder.Default.Encode(error)}</li>");
      }
      sb.Append("</ul>");
      sb.Append("</div>");
      sb.Append(@"
                <button type=""button"" class=""position-absolute top-0 end-0 btn btn-icon ms-sm-auto"" data-bs-dismiss=""alert"">
                    <i class=""ki-duotone ki-cross fs-1 text-light"">
                        <span class=""path1""></span><span class=""path2""></span>
                    </i>
                </button>");
      output.Content.SetHtmlContent(sb.ToString());
   }
}
