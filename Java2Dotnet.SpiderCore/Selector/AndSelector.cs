using System.Collections.Generic;

namespace Java2Dotnet.Spider.Core.Selector
{
	public class AndSelector : ISelector
	{
		private readonly IList<ISelector> _selectors = new List<ISelector>();

		public AndSelector(params ISelector[] selectors)
		{
			foreach (ISelector selector in selectors)
			{
				_selectors.Add(selector);
			}
		}

		public AndSelector(IList<ISelector> selectors)
		{
			_selectors = selectors;
		}

		public string Select(string text)
		{
			foreach (ISelector selector in _selectors)
			{
				if (text == null)
				{
					return null;
				}
				text = selector.Select(text);
			}
			return text;
		}

		public IList<string> SelectList(string text)
		{
			IList<string> results = new List<string>();
			bool first = true;
			foreach (ISelector selector in _selectors)
			{
				if (first)
				{
					results = selector.SelectList(text);
					first = false;
				}
				else
				{
					List<string> resultsTemp = new List<string>();
					foreach (string result in results)
					{
						resultsTemp.AddRange(selector.SelectList(result));
					}
					results = resultsTemp;
					if (results.Count == 0)
					{
						return  results;
					}
				}
			}
			return results;
		}
	}
}
