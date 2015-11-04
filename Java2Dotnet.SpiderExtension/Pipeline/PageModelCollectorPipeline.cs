using System;
using System.Collections;
using System.Collections.Generic;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Pipeline;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	internal sealed class PageModelCollectorPipeline : ICollectorPipeline
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
			if (resultItems == null)
			{
				return;
			}

			Dictionary<Type, List<dynamic>> resultDictionary = new Dictionary<Type, List<dynamic>>();

			dynamic data = resultItems.Get(_type.FullName);

			if (typeof(IEnumerable).IsAssignableFrom(_type))
			{
				if (resultDictionary.ContainsKey(_type))
				{
					resultDictionary[_type].AddRange(data);
				}
				else
				{
					List<dynamic> list = new List<dynamic>();
					list.AddRange(data);
					resultDictionary.Add(_type, list);
				}
			}
			else
			{
				if (resultDictionary.ContainsKey(_type))
				{
					resultDictionary[_type].Add(data);
				}
				else
				{
					resultDictionary.Add(_type, new List<dynamic> { data });
				}
			}

			_collectorPipeline.Process(resultDictionary, task);
		}
	}
}
