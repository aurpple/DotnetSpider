using System;
using System.Collections.Generic;

namespace Java2Dotnet.Spider.Core.Selector
{
	public class HtmlNode : AbstractSelectable
	{
		protected virtual IList<HtmlAgilityPack.HtmlNode> Elements { get; }

		public HtmlNode(IList<HtmlAgilityPack.HtmlNode> elements)
		{
			Elements = elements;
		}

		public HtmlNode()
		{
			Elements = null;
		}

		public override ISelectable Links()
		{
			return XPath("//a/@href");
		}

		public override ISelectable XPath(string xpath)
		{
			XPathSelector xpathSelector = Selectors.XPath(xpath);
			return SelectElements(xpathSelector);
		}

		protected override IList<string> GetSourceTexts()
		{
			IList<string> sourceTexts = new List<string>(Elements.Count);
			foreach (HtmlAgilityPack.HtmlNode element in Elements)
			{
				sourceTexts.Add(element.OuterHtml);
			}
			return sourceTexts;
		}

		/// <summary>
		/// Select elements
		/// </summary>
		/// <param name="elementSelector"></param>
		/// <returns></returns>
		protected ISelectable SelectElements(BaseElementSelector elementSelector)
		{
			var elementIterator = Elements;
			if (!elementSelector.HasAttribute())
			{
				List<HtmlAgilityPack.HtmlNode> resultElements = new List<HtmlAgilityPack.HtmlNode>();
				foreach (var element in elementIterator)
				{
					HtmlAgilityPack.HtmlNode node = CheckElementAndConvert(element);
					IList<HtmlAgilityPack.HtmlNode> selectElements = elementSelector.SelectElements(node);
					resultElements.AddRange(selectElements);
				}
				return new HtmlNode(resultElements);
			}
			else
			{
				// has attribute, consider as plaintext
				List<string> resultStrings = new List<string>();
				foreach (var element in elementIterator)
				{
					HtmlAgilityPack.HtmlNode node = CheckElementAndConvert(element);
					IList<string> selectList = elementSelector.SelectList(node);
					resultStrings.AddRange(selectList);
				}
				return new PlainText(resultStrings);
			}
		}

		public override ISelectable Css(string selector)
		{
			CssSelector cssSelector = Selectors.Css(selector);
			return SelectElements(cssSelector);
		}

		public override ISelectable Css(string selector, string attrName)
		{
			var cssSelector = Selectors.Css(selector, attrName);
			return SelectElements(cssSelector);
		}

		public override IList<ISelectable> Nodes()
		{
			IList<ISelectable> selectables = new List<ISelectable>();
			foreach (HtmlAgilityPack.HtmlNode element in Elements)
			{
				IList<HtmlAgilityPack.HtmlNode> childElements = new List<HtmlAgilityPack.HtmlNode>(1);
				childElements.Add(element);
				selectables.Add(new HtmlNode(childElements));
			}
			return selectables;
		}

		public override ISelectable SmartContent()
		{
			SmartContentSelector smartContentSelector = Selectors.SmartContent();
			return Select(smartContentSelector, GetSourceTexts());
		}

		/// <summary>
		/// Only document can be select
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		private HtmlAgilityPack.HtmlNode CheckElementAndConvert(object element)
		{
			HtmlAgilityPack.HtmlNode node = element as HtmlAgilityPack.HtmlNode;
			if (node != null)
			{
				if (node.NodeType == HtmlAgilityPack.HtmlNodeType.Document)
				{
					return node;
				}
				HtmlAgilityPack.HtmlDocument root = new HtmlAgilityPack.HtmlDocument();
				root.DocumentNode.ChildNodes.Append(node.CloneNode(true));
				return root.DocumentNode;
			}
			throw new ArgumentException("Element is not HtmlNode");
		}
	}
}