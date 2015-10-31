using System;
using System.Collections;
using System.Collections.Generic;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Pipeline;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	public sealed class PageModelCollectorPipeline : ICollectorPipeline
	{
		private readonly CollectorPageModelPipeline _collectorPipeline = new CollectorPageModelPipeline();
		private readonly Type _type;

		public PageModelCollectorPipeline(Type type)
		{
			_type = type;
		}

		public ICollection GetCollected()
		{
			return _collectorPipeline.GetCollected();
		}

		//[MethodImplAttribute(MethodImplOptions.Synchronized)]
		public void Process(ResultItems resultItems, ITask task)
		{
			dynamic o = resultItems.Get(_type.FullName);
			if (o != null)
			{
				//check
				_collectorPipeline.Process(o, task);
			}
		}
	}
}
