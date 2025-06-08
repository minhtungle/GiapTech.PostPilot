using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Public.Helpers
{
    public class HtmlHelper
    {
        public static IHtmlString RenderToggleLinks(string tableId, string[] headers)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < headers.Length; i++)
            {
                sb.AppendFormat("<a href=\"#\" class=\"toggle-vis\" data-table=\"{0}\" data-column=\"{1}\">{2}</a>",
                    tableId,
                    i + 1,
                    HttpUtility.HtmlEncode(headers[i]) // để an toàn với XSS
                );

                if (i < headers.Length - 1)
                    sb.Append(" | ");
            }

            return new MvcHtmlString(sb.ToString());
        }
    }
}