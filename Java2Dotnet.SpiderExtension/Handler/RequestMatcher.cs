using Java2Dotnet.Spider.Core;

namespace Java2Dotnet.Spider.Extension.Handler
{
	public enum MatchOther
	{
		Yes, No
	}

	public interface IRequestMatcher
	{
		/// <summary>
		/// Check whether to process the page.
		/// Please DO NOT change page status in this method.
		/// </summary>
		/// <param name="page"></param>
		/// <returns></returns>
		bool Match(Request page);
	}
}