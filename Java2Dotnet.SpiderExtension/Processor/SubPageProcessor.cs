using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Extension.Handler;

namespace Java2Dotnet.Spider.Extension.Processor
{
	public interface ISubPageProcessor : IRequestMatcher
	{
		/// <summary>
		/// Process the page, extract urls to fetch, extract the data and store
		/// </summary>
		/// <param name="page"></param>
		/// <returns></returns>
		MatchOther ProcessPage(Page page);
	}
}