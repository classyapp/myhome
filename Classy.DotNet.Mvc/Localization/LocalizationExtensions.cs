using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Classy.DotNet.Mvc.Localization
{
    public static class LocalizationExtensions
    {
        public static IEnumerable<LocalizedListItem> WithParent(this IEnumerable<LocalizedListItem> list, string parentValue)
        {
            return list.Where(x => x.ParentValue == parentValue);
        }

        public static SelectList AsSelectList(this IEnumerable<LocalizedListItem> list)
        {
            return new SelectList(list, "Value", "Text");
        }

        public static SelectList AsSelectList(this IEnumerable<LocalizedListItem> list, string[] blacklist)
        {
            return new SelectList(list.Where(l => !blacklist.Contains(l.Value)), "Value", "Text");
        }
    }
}
