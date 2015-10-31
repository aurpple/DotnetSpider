using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
				Dictionary<Type, List<dynamic>> resultDictionary = new Dictionary<Type, List<dynamic>>();
				foreach (var resultItems in resultItemsList)
				{
					dynamic data = resultItems.Get(pipelineEntry.Key.FullName);
					Type type = data.GetType();

					if (typeof(IEnumerable).IsAssignableFrom(type))
					{
						if (resultDictionary.ContainsKey(type))
						{
							resultDictionary[type].AddRange(data);
						}
						else
						{
							List<dynamic> list = new List<dynamic>();
							list.AddRange(data);
							resultDictionary.Add(type, list);
						}
					}
					else
					{
						if (resultDictionary.ContainsKey(type))
						{
							resultDictionary[type].Add(data);
						}
						else
						{
							resultDictionary.Add(type, new List<dynamic> { data });
						}
					}
				}
				pipelineEntry.Value.Process(resultDictionary, task);
			}
		}
	}
}