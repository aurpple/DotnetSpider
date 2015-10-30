using System.Collections.Generic;
using System.Runtime.CompilerServices;
using log4net;

namespace Java2Dotnet.Spider.Core.Pipeline
{
	public delegate void FlushCachedPipeline(ITask task);

	public abstract class CachedPipeline : IPipeline
	{
		protected static readonly ILog Logger = LogManager.GetLogger(typeof(CachedPipeline));

		private readonly List<ResultItems> _cached = new List<ResultItems>();

		public int CachedSize { get; set; } = 1000;

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Process(ResultItems resultItems, ITask task)
		{
			_cached.Add(resultItems);

			if (_cached.Count >= CachedSize)
			{
				// 做成异步
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
