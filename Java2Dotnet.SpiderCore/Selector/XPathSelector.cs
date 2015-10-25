using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Java2Dotnet.Spider.Core.Selector
{
	public class XPathSelector : BaseElementSelector
	{
		private readonly string _xpath;
		private static readonly Regex AttributeXPathRegex = new Regex(@"@[\w\s-]+", RegexOptions.RightToLeft | RegexOptions.IgnoreCase);
		private readonly string _attribute;

		public XPathSelector(string xpathStr)
		{
			_xpath = xpathStr;
			//if (!string.IsNullOrEmpty(this.xpath))
			//{
			Match match = AttributeXPathRegex.Match(_xpath);
			if (_xpath.EndsWith(match.Value))
			{
				_attribute = match.Value.Replace("@", "");
			}
			//}
		}

		public override string Select(HtmlAgilityPack.HtmlNode element)
		{
			var node = element.SelectSingleNode(_xpath);
			if (node != null)
			{
				return HasAttribute() ? node.Attributes[_attribute].Value?.Trim() : node.InnerHtml?.Trim();
			}
			return null;
		}

		public override IList<string> SelectList(HtmlAgilityPack.HtmlNode element)
		{
			List<string> result = new List<string>();
			var nodes = element.SelectNodes(_xpath);
			if (nodes != null)
			{
				foreach (var node in nodes)
				{
					if (!HasAttribute())
					{
						result.Add(node.OuterHtml?.Trim());
					}
					else
					{
						var attr = node.Attributes[_attribute];
						if (attr != null)
						{
							result.Add(attr.Value?.Trim());
						}
					}
				}
			}
			return result;
		}

		public override HtmlAgilityPack.HtmlNode SelectElement(HtmlAgilityPack.HtmlNode element)
		{
			IList<HtmlAgilityPack.HtmlNode> elements = SelectElements(element);
			if (elements != null && elements.Count > 0)
			{
				return elements[0];
			}
			return null;
		}

		public override IList<HtmlAgilityPack.HtmlNode> SelectElements(HtmlAgilityPack.HtmlNode element)
		{
			return element.SelectNodes(_xpath);
		}

		public override bool HasAttribute()
		{
			return !string.IsNullOrEmpty(_attribute);
		}
	}
}
