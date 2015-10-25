using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Extension.Handler;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	public interface ISubPipeline : IRequestMatcher
	{
		/// <summary>
		/// Process the page, extract urls to fetch, extract the data and store
		/// </summary>
		/// <param name="resultItems"></param>
		/// <param name="task"></param>
		/// <returns></returns>
		MatchOther ProcessResult(ResultItems resultItems, ITask task);
	}
}