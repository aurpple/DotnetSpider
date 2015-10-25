using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;

namespace Java2Dotnet.Spider.Core.Selector
{
	public class CssSelector : BaseElementSelector
	{
		private readonly string _selectorText;
		private readonly string _attrName;

		public CssSelector(string selectorText)
		{
			_selectorText = selectorText;
		}

		public CssSelector(string selectorText, string attrName)
		{
			_selectorText = selectorText;
			_attrName = attrName;
		}

		protected string GetText(HtmlAgilityPack.HtmlNode element)
		{
			StringBuilder accum = new StringBuilder();
			foreach (var node in element.ChildNodes)
			{
				if (node is HtmlTextNode)
				{
					accum.Append(node.InnerText);
				}
			}
			return accum.ToString();
		}

		public override string Select(HtmlAgilityPack.HtmlNode element)
		{
			IList<HtmlAgilityPack.HtmlNode> elements = SelectElements(element);
			if (elements == null || elements.Count == 0)
			{
				return null;
			}
			return GetValue(elements[0]);
		}

		public override IList<string> SelectList(HtmlAgilityPack.HtmlNode doc)
		{
			IList<string> strings = new List<string>();
			IList<HtmlAgilityPack.HtmlNode> elements = SelectElements(doc);
			if (elements != null && elements.Count > 0)
			{
				foreach (HtmlAgilityPack.HtmlNode element in elements)
				{
					string value = GetValue(element);
					if (value != null)
					{
						strings.Add(value);
					}
				}
			}
			return strings;
		}

		public override HtmlAgilityPack.HtmlNode SelectElement(HtmlAgilityPack.HtmlNode element)
		{
			IList<HtmlAgilityPack.HtmlNode> elements = element.QuerySelectorAll(_selectorText);
			if (elements != null && elements.Count > 0)
			{
				return elements[0];
			}
			return null;
		}

		public override IList<HtmlAgilityPack.HtmlNode> SelectElements(HtmlAgilityPack.HtmlNode element)
		{
			return element.QuerySelectorAll(_selectorText);
		}

		public override bool HasAttribute()
		{
			return _attrName != null;
		}

		private string GetValue(HtmlAgilityPack.HtmlNode element)
		{
			if (_attrName == null)
			{
				return element.OuterHtml;
			}
			else if ("innerhtml".Equals(_attrName.ToLower()))
			{
				return element.InnerHtml;
			}
			else if ("text".Equals(_attrName.ToLower()))
			{
				return GetText(element);
			}
			//check
			//else if ("allText".equalsIgnoreCase(attrName)) {
			//	return element.text();
			//} 
			else
			{
				return element.Attributes[_attrName].Value;
			}
		}
	}
}
