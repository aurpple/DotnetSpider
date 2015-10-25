using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Java2Dotnet.Spider.Core.Pipeline
{
	public class ResultItemsCollectorPipeline : ICollectorPipeline
	{
		// memory will not enough if this list is too large?
		private readonly List<ResultItems> _collector = new List<ResultItems>();

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Process(ResultItems resultItems, ITask task)
		{
			_collector.Add(resultItems);
		}

		public ICollection GetCollected()
		{
			return _collector;
		}
	}
}
