using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Pipeline;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	/// <summary>
	/// The extension to Pipeline for page model extractor.
	/// </summary>
	public class ModelPipeline : CachedPipeline
	{
		private readonly ConcurrentDictionary<Type, IPageModelPipeline> _pageModelPipelines = new ConcurrentDictionary<Type, IPageModelPipeline>();

		public ModelPipeline Put(Type type, IPageModelPipeline pageModelPipeline)
		{
			_pageModelPipelines.TryAdd(type, pageModelPipeline);
			return this;
		}

		protected override void Process(List<ResultItems> resultItemsList, ITask task)
		{
			if (resultItemsList == null || resultItemsList.Count == 0)
			{
				return;
			}

			foreach (var pipelineEntry in _pageModelPipelines)
			{
				List<dynamic> valueList = new List<dynamic>();
				foreach (var resultItems in resultItemsList)
				{
					dynamic data = resultItems.Get(pipelineEntry.Key.FullName);
					if (data.GetType().IsGenericType)
					{
						valueList.AddRange(data);
					}
					else
					{
						valueList.Add(data);
					}
				}
				pipelineEntry.Value.Process(valueList, task);
			}
		}
	}
}