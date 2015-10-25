using System;
using System.Collections.Generic;
using System.Linq;

namespace Java2Dotnet.Spider.Core.Selector
{
	/// <summary>
	/// Selectable plain text. 
	/// Can not be selected by XPath or CSS Selector.
	/// </summary>
	public class PlainText : AbstractSelectable
	{
		protected IList<string> SourceTexts;

		public PlainText(IList<string> sourceTexts)
		{
			SourceTexts = sourceTexts;
		}

		public PlainText(string text)
		{
			SourceTexts = new List<string>() { text };
		}

		public static PlainText Create(string text)
		{
			return new PlainText(text);
		}

		public override ISelectable XPath(string xpath)
		{
			XPathSelector xpathSelector = Selectors.XPath(xpath);
			return SelectList(xpathSelector);
		}

		public override ISelectable SmartContent()
		{
			throw new NotImplementedException();
		}

		public override ISelectable Links()
		{
			return XPath("//a/@href");
		}

		public override IList<ISelectable> Nodes()
		{
			List<ISelectable> nodes = new List<ISelectable>(GetSourceTexts().Count);
			nodes.AddRange(GetSourceTexts().Select(Create));
			return nodes;
		}

		protected override IList<string> GetSourceTexts()
		{
			return SourceTexts;
		}

		public override ISelectable Css(string selector)
		{
			CssSelector cssSelector = Selectors.Css(selector);
			return Select(cssSelector);
		}

		public override ISelectable Css(string selector, string attrName)
		{
			CssSelector cssSelector = Selectors.Css(selector, attrName);
			return Select(cssSelector);
		}

		public override ISelectable JsonPath(string jsonPath)
		{
			throw new NotImplementedException();
		}
	}
}