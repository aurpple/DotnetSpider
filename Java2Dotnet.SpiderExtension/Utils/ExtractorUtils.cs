using System.Collections.Generic;
using Java2Dotnet.Spider.Core.Selector;
using Java2Dotnet.Spider.Extension.Model;
using Java2Dotnet.Spider.Extension.Model.Attribute;

namespace Java2Dotnet.Spider.Extension.Utils
{
	/// <summary>
	/// Tools for annotation converting. 
	/// </summary>
	public class ExtractorUtils
	{
		public static ISelector GetSelector(ExtractBy extractBy)
		{
			string value = extractBy.Value;
			ISelector selector;
			switch (extractBy.Type)
			{
				case ExtractBy.ExtracType.Css:
					selector = new CssSelector(value);
					break;
				case ExtractBy.ExtracType.Regex:
					selector = new RegexSelector(value);
					break;
				case ExtractBy.ExtracType.XPath:
					selector = GetXpathSelector(value);
					break;
				case ExtractBy.ExtracType.JsonPath:
					selector = new JsonPathSelector(value);
					break;
				case ExtractBy.ExtracType.Enviroment:
					selector = new EnviromentSelector(value);
					break;
				default:
					selector = GetXpathSelector(value);
					break;
			}
			return selector;
		}

		public static Extractor GetExtractor(ExtractBy extractBy)
		{
			ISelector selector = GetSelector(extractBy);
			return new Extractor(selector, extractBy.Source, extractBy.NotNull, extractBy.Multi, extractBy.Expresion,extractBy.Count);
		}

		private static ISelector GetXpathSelector(string value)
		{
			ISelector selector = new XPathSelector(value);
			return selector;
		}

		public static IList<ISelector> GetSelectors(ExtractBy[] extractBies)
		{
			IList<ISelector> selectors = new List<ISelector>();
			if (extractBies == null)
			{
				return selectors;
			}
			foreach (ExtractBy extractBy in extractBies)
			{
				selectors.Add(GetSelector(extractBy));
			}
			return selectors;
		}
	}
}