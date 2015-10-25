using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using HtmlAgilityPack;
using Java2Dotnet.Spider.Core.Utils;
using log4net;

namespace Java2Dotnet.Spider.Core.Selector
{
	/// <summary>
	/// Selectable html.
	/// </summary>
	public class Html : HtmlNode
	{
		private static readonly ILog Logger = LogManager.GetLogger(typeof(Html));

		/// <summary>
		/// Store parsed document for better performance when only one text exist.
		/// </summary>
		public HtmlAgilityPack.HtmlNode Document { get; }

		public Html(string text, string url = null)
		{
			try
			{
				HtmlDocument document = new HtmlDocument();
				document.LoadHtml(text);
				Document = document.DocumentNode;

				if (!string.IsNullOrEmpty(url))
				{
					FixAllRelativeHrefs(url);
				}
			}
			catch (Exception e)
			{
				Document = null;
				Logger.Warn("parse document error ", e);
			}
		}

		public Html(HtmlAgilityPack.HtmlNode document)
		{
			Document = document;
		}

		protected override IList<HtmlAgilityPack.HtmlNode> Elements => new ReadOnlyCollection<HtmlAgilityPack.HtmlNode>(new List<HtmlAgilityPack.HtmlNode> { Document });

		public string SelectDocument(ISelector selector)
		{
			IElementSelector elementSelector = selector as IElementSelector;
			if (elementSelector != null)
			{
				return elementSelector.Select(Document);
			}
			return selector?.Select(GetFirstSourceText());
		}

		public IList<string> SelectDocumentForList(ISelector selector)
		{
			var elementSelector = selector as IElementSelector;
			if (elementSelector != null)
			{
				return elementSelector.SelectList(Document);
			}
			return selector?.SelectList(GetFirstSourceText());
		}

		// 问题太多, 如果有需要移到实体类的Expression中处理
		internal void FixAllRelativeHrefs(string url)
		{
			var nodes = Document.SelectNodes("//a[not(starts-with(@href,'http') or starts-with(@href,'https'))]");
			if (nodes != null)
			{
				foreach (var node in nodes)
				{
					if (node.Attributes["href"] != null)
					{
						node.Attributes["href"].Value = UrlUtils.CanonicalizeUrl(node.Attributes["href"].Value, url);
					}
				}
			}
		}

		public static Html Create(string text)
		{
			return new Html(text);
		}
	}
}
