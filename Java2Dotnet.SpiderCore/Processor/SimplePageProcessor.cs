using System.Collections.Generic;
using Java2Dotnet.Spider.Core.Utils;

namespace Java2Dotnet.Spider.Core.Processor
{
	/// <summary>
	/// A simple PageProcessor.
	/// </summary>
	public class SimplePageProcessor : IPageProcessor
	{
		private readonly string _urlPattern;

		public SimplePageProcessor(string startUrl, string urlPattern)
		{
			Site = new Site();  //test
			Site.AddStartUrl(startUrl);
			Site.Domain = UrlUtils.GetDomain(startUrl);
			//compile "*" expression to regex
			_urlPattern = "(" + urlPattern.Replace(".", "\\.").Replace("*", "[^\"'#]*") + ")";
		}

		public void Process(Page page)
		{
			IList<string> requests = page.GetHtml().Links().Regex(_urlPattern).GetAll();
			//add urls to fetch
			page.AddTargetRequests(requests);
			//extract by XPath
			page.PutField("title", page.GetHtml().XPath("//title"));
			page.PutField("html", page.GetHtml().ToString());
			//extract by Readability
			page.PutField("content", page.GetHtml().SmartContent());
		}

		/// <summary>
		/// Get the site settings
		/// </summary>
		public Site Site { get; }
	}
}
