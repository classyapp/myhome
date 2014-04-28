using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.Helpers
{
    public class Html2Markdown
    {
        private readonly IList<Element> _elements = new List<Element>
			{
				new Element{
					Pattern = @"<a.+?href\s*=\s*['""]([^'""]+)['""]>([^<]+)</a>",
					Replacement = @"[$2]($1)"
				},
				new Element{
					Pattern = @"</?(strong|b)>",
					Replacement = @"**"
				},
				new Element{
					Pattern = @"</?(em|i)>",
					Replacement = @"_"
				},
				new Element{
					Pattern = @"<br\s?/?>",
					Replacement = @"  " + System.Environment.NewLine
				},
				new Element{
					Pattern = @"</?code>",
					Replacement = @"`"
				},
				new Element{
					Pattern = @"</h[1-6]>",
					Replacement = System.Environment.NewLine + System.Environment.NewLine
				},
				new Element{
					Pattern = @"<h1>",
					Replacement = System.Environment.NewLine + System.Environment.NewLine + "# "
				},
				new Element{
					Pattern = @"<h2>",
					Replacement = System.Environment.NewLine + System.Environment.NewLine + "## "
				},
				new Element{
					Pattern = @"<h3>",
					Replacement = System.Environment.NewLine + System.Environment.NewLine + "### "
				},
				new Element{
					Pattern = @"<h4>",
					Replacement = System.Environment.NewLine + System.Environment.NewLine + "#### "
				},
				new Element{
					Pattern = @"<h5>",
					Replacement = System.Environment.NewLine + System.Environment.NewLine + "##### "
				},
				new Element{
					Pattern = @"<h6>",
					Replacement = System.Environment.NewLine + System.Environment.NewLine + "###### "
				},
                new Element{
                    Pattern = @"<blockquote>",
                    Replacement = System.Environment.NewLine + System.Environment.NewLine + @">"
                },
                new Element{
                    Pattern = @"</?blockquote>",
                    Replacement = System.Environment.NewLine + System.Environment.NewLine
                },
                new Element{
					Pattern = @"</?(div|p)>",
					Replacement = @"  " + System.Environment.NewLine
				},
                new Element{
					Pattern = @"</?span>",
					Replacement = @"  "
				}
			};

        public string Convert(string html)
        {
            return _elements.Aggregate(html, (current, element) => ReplacePattern(current, element.Pattern, element.Replacement));
        }

        private static string ReplacePattern(string html, string pattern, string replacement)
        {
            var regex = new Regex(pattern);

            return html == null ? string.Empty : regex.Replace(html, replacement);
        }
    }
    internal class Element
    {
        public string Pattern { get; set; }

        public string Replacement { get; set; }
    }
}
