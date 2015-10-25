using System;
using System.Collections;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Pipeline;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	public class PageModelCollectorPipeline : ICollectorPipeline
	{
		private readonly CollectorPageModelToDbPipeline _classPipeline = new CollectorPageModelToDbPipeline();
		private readonly Type _type;

		public PageModelCollectorPipeline(Type type)
		{
			_type = type;
		}

		public ICollection GetCollected()
		{
			return _classPipeline.GetCollected();
		}

		//[MethodImplAttribute(MethodImplOptions.Synchronized)]
		public virtual void Process(ResultItems resultItems, ITask task)
		{
			dynamic o = resultItems.Get(_type.FullName);
			if (o != null)
			{
				//check
				_classPipeline.Process(o, task);
			}
		}
	}
}
