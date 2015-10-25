using System.Collections.Generic;
using HtmlAgilityPack;

namespace Java2Dotnet.Spider.Core.Selector
{
	public abstract class BaseElementSelector : ISelector, IElementSelector
	{
		public virtual string Select(string text)
		{
			if (text != null)
			{
				HtmlDocument document = new HtmlDocument();
				document.LoadHtml(text);
				return Select(document.DocumentNode);
			}
			return null;
		}

		public virtual IList<string> SelectList(string text)
		{
			if (text != null)
			{
				HtmlDocument document = new HtmlDocument();
				document.LoadHtml(text);
				return SelectList(document.DocumentNode);
			}
			else
			{
				return new List<string>();
			}
		}

		public virtual HtmlAgilityPack.HtmlNode SelectElement(string text)
		{
			if (text != null)
			{
				HtmlDocument document = new HtmlDocument();
				document.LoadHtml(text);
				return SelectElement(document.DocumentNode);
			}
			return null;
		}

		public virtual IList<HtmlAgilityPack.HtmlNode> SelectElements(string text)
		{
			if (text != null)
			{
				HtmlDocument document = new HtmlDocument();
				document.LoadHtml(text);
				return SelectElements(document.DocumentNode);
			}
			else
			{
				return new List<HtmlAgilityPack.HtmlNode>();
			}
		}

		public abstract HtmlAgilityPack.HtmlNode SelectElement(HtmlAgilityPack.HtmlNode element);
		public abstract IList<HtmlAgilityPack.HtmlNode> SelectElements(HtmlAgilityPack.HtmlNode element);
		public abstract bool HasAttribute();
		public abstract string Select(HtmlAgilityPack.HtmlNode element);
		public abstract IList<string> SelectList(HtmlAgilityPack.HtmlNode element);
	}
}