using System;
using System.Collections.Generic;
using System.Linq;

namespace Java2Dotnet.Spider.Core.Selector
{
	public abstract class AbstractSelectable : ISelectable
	{
		protected abstract IList<string> GetSourceTexts();
		public abstract ISelectable XPath(string xpath);
		public abstract ISelectable Css(string selector);
		public abstract ISelectable Css(string selector, string attrName);
		public abstract ISelectable SmartContent();
		public abstract ISelectable Links();
		public abstract IList<ISelectable> Nodes();

		public ISelectable Select(ISelector selector, IList<string> strings)
		{
			IList<string> results = strings.Select(selector.Select).Where(result => result != null).ToList();
			return new PlainText(results);
		}

		protected ISelectable SelectList(ISelector selector, IList<string> strings)
		{
			List<string> results = new List<string>();
			foreach (string str in strings)
			{
				IList<string> result = selector.SelectList(str);
				results.AddRange(result);
			}
			return new PlainText(results);
		}

		public IList<string> GetAll()
		{
			return GetSourceTexts();
		}

		public string Value
		{
			get
			{
				if (GetAll() != null && GetAll().Count > 0)
				{
					return GetAll()[0];
				}
				else
				{
					return null;
				}
			}
		}

		public ISelectable Select(ISelector selector)
		{
			return Select(selector, GetSourceTexts());
		}

		public ISelectable SelectList(ISelector selector)
		{
			return SelectList(selector, GetSourceTexts());
		}

		public ISelectable Regex(string regex)
		{
			RegexSelector regexSelector = Selectors.Regex(regex);
			return SelectList(regexSelector, GetSourceTexts());
		}

		public ISelectable Regex(string regex, int group)
		{
			RegexSelector regexSelector = Selectors.Regex(regex, group);
			return SelectList(regexSelector, GetSourceTexts());
		}

		public ISelectable Replace(string regex, string replacement)
		{
			ReplaceSelector replaceSelector = new ReplaceSelector(regex, replacement);
			return Select(replaceSelector, GetSourceTexts());
		}

		public string GetFirstSourceText()
		{
			if (GetSourceTexts() != null && GetSourceTexts().Count > 0)
			{
				return GetSourceTexts()[0];
			}
			return null;
		}

		public override string ToString()
		{
			return Value;
		}

		public bool Exist()
		{
			return GetSourceTexts() != null && GetSourceTexts().Count > 0;
		}

		public virtual ISelectable JsonPath(string jsonPath)
		{
			throw new NotImplementedException();
		}
	}
}