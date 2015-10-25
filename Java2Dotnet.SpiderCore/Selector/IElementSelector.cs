using System.Collections.Generic;

namespace Java2Dotnet.Spider.Core.Selector
{
	/// <summary>
	/// Selector(extractor) for html elements.
	/// </summary>
	public interface IElementSelector
	{
		/// <summary>
		/// Extract single result in text. 
		/// If there are more than one result, only the first will be chosen.
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		string Select(HtmlAgilityPack.HtmlNode element);

		/// <summary>
		/// Extract all results in text.
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		IList<string> SelectList(HtmlAgilityPack.HtmlNode element);
	}
}
