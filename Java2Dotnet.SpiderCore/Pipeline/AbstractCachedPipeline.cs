using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Java2Dotnet.Spider.Core.Pipeline
{
	public delegate void Flush(ITask task);

	public abstract class AbstractCachedPipeline : IPipeline
	{
		private readonly List<ResultItems> _cached = new List<ResultItems>();

		public int CachedSize { get; set; } = 1000;

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Process(ResultItems resultItems, ITask task)
		{
			_cached.Add(resultItems);

			if (_cached.Count > CachedSize)
			{
				Process(_cached, task);
				_cached.Clear();
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Flush(ITask task)
		{
			if (_cached.Count > 0)
			{
				Process(_cached, task);
				_cached.Clear();
			}
		}

		protected abstract void Process(List<ResultItems> resultItemsList, ITask task);
	}
}
