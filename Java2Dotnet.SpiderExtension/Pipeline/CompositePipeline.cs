using System.Collections.Generic;
using System.Linq;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Pipeline;
using Java2Dotnet.Spider.Extension.Handler;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	public class CompositePipeline : IPipeline
	{
		private IList<ISubPipeline> _subPipelines = new List<ISubPipeline>();

		public void Process(ResultItems resultItems, ITask task)
		{
			if ((from subPipeline in _subPipelines where subPipeline.Match(resultItems.Request) select subPipeline.ProcessResult(resultItems, task)).Any(matchOtherProcessorProcessor => matchOtherProcessorProcessor != MatchOther.Yes))
			{
			}
		}

		public CompositePipeline AddSubPipeline(ISubPipeline subPipeline)
		{
			_subPipelines.Add(subPipeline);
			return this;
		}

		public CompositePipeline SetSubPipeline(params ISubPipeline[] subPipelines)
		{
			_subPipelines = new List<ISubPipeline>();
			foreach (ISubPipeline subPipeline in subPipelines)
			{
		 		_subPipelines.Add(subPipeline);
			}
			return this;
		}
	}
}
