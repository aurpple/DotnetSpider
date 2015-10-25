using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Extension.Handler;
using Java2Dotnet.Spider.Extension.Pipeline;

namespace Java2Dotnet.Spider.Extension.Processor
{
	public abstract class PatternProcessor : PatternRequestMatcher, ISubPipeline, ISubPageProcessor
	{
		protected PatternProcessor(string pattern)
			: base(pattern)
		{
		}

		public abstract MatchOther ProcessResult(ResultItems resultItems, ITask task);
		public abstract MatchOther ProcessPage(Page page);
	}
}
