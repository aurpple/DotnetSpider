using System.Collections.Generic;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Processor;
using Java2Dotnet.Spider.Core.Utils;

namespace Java2Dotnet.Spider.Extension.Configurable
{
	[Experimental]
	public class ConfigurablePageProcessor : IPageProcessor
	{
		private readonly IList<ExtractRule> _extractRules;

		public ConfigurablePageProcessor(Site site, List<ExtractRule> extractRules)
		{
			Site = site;
			_extractRules = extractRules;
		}

		public void Process(Page page)
		{
			foreach (ExtractRule extractRule in _extractRules)
			{
				if (extractRule.IsMulti)
				{
					IList<string> results = page.GetHtml().SelectDocumentForList(extractRule.Selector);
					if (extractRule.IsNotNull && results.Count == 0)
					{
						page.SetSkip(true);
					}
					else
					{
						page.GetResultItems().Put(extractRule.FieldName, results);
					}
				}
				else
				{
					string result = page.GetHtml().SelectDocument(extractRule.Selector);
					if (extractRule.IsNotNull && result == null)
					{
						page.SetSkip(true);
					}
					else
					{
						page.GetResultItems().Put(extractRule.FieldName, result);
					}
				}
			}
		}

		public Site Site { get; }
	}
}