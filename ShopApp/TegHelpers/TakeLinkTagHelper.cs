using Microsoft.AspNetCore.Razor.TagHelpers;
using ShopApp.WebUI.Models;
using System.Text;

namespace ShopApp.WebUI.TegHelpers
{

    [HtmlTargetElement("div", Attributes = "page-model")]
    public class TakeLinkTagHelper : TagHelper
    {
        public PageInfo PageModel { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            StringBuilder stringBuilder = new StringBuilder();

            if (PageModel.CurrentPage > 1)
            {
                if (string.IsNullOrEmpty(PageModel.CurrenCategory))
                {
                    stringBuilder.AppendFormat("<a href='/AllList?page={0}'>«</a>", PageModel.CurrentPage - 1);
                }
                else
                {
                    stringBuilder.AppendFormat("<a href='/AllList/{1}?page={0}'>«</a>", PageModel.CurrentPage - 1, PageModel.CurrenCategory);
                }
            }

            for (int i = 1; i <= PageModel.TotalPages(); i++)
            {
                if (string.IsNullOrEmpty(PageModel.CurrenCategory))
                {
                    stringBuilder.AppendFormat("<a href='/AllList?page={0}'>{0}</a>", i);
                }
                else
                {
                    stringBuilder.AppendFormat("<a href='/AllList/{1}?page={0}'>{0}</a>", i, PageModel.CurrenCategory);
                }
            }

            if (PageModel.CurrentPage < PageModel.TotalPages())
            {
                if (string.IsNullOrEmpty(PageModel.CurrenCategory))
                {
                    stringBuilder.AppendFormat("<a href='/AllList?page={0}'>»</a>", PageModel.CurrentPage + 1);
                }
                else
                {
                    stringBuilder.AppendFormat("<a href='/AllList/{1}?page={0}'>»</a>", PageModel.CurrentPage + 1, PageModel.CurrenCategory);
                }
            }

            output.Content.SetHtmlContent(stringBuilder.ToString());
            base.Process(context, output);
        }
    }
}
